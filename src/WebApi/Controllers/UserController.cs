using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.User;
using WebApi.DTOs.Responses.User;
using WebApi.Models.Aggregates;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Implementations;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v1/users/me")]
public class UserController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupMemberRepository groupMemberRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRoleRepository userRoleRepository,
    IUserPreferenceRepository userPreferenceRepository,
    IGroupRepository groupRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IRedisService redisService): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        try
        {
            var _ = new UserPreference(user.Preferences);

            return Ok(new GetCurrentUserResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = user.Preferences is not null ? 
                    new GetCurrentUserPreferenceResponse
                    {
                        LikesCommercial = user.Preferences.LikesCommercial,
                        Food = user.Preferences.Food,
                        Culture = user.Preferences.Culture,
                        Entertainment = user.Preferences.Entertainment,
                        PlaceTypes = user.Preferences.PlaceTypes,
                    } : null,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(
                new ErrorResponse("Invalid preferences are filled for current user, please contact support."));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateCurrentUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        // TODO: parou de funcionar
        userRepository.Remove(user);
        await unitOfWork.SaveAsync();

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", user.Id.ToString());
        await redisService.DeleteAsync(key);

        return NoContent();
    }

    [HttpPatch("anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser()
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        groupMemberRepository.RemoveRange(user.GroupMemberships);
        userGroupInvitationRepository.RemoveRange(user.AcceptedInvitations);
        userRoleRepository.RemoveRange(user.UserRoles);

        user.Anonymize();

        userRepository.Update(user);
        await unitOfWork.SaveAsync();

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", user.Id.ToString());
        await redisService.DeleteAsync(key);

        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> SetCurrentUserPreferences(
        [FromBody] SetCurrentUserPreferencesRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        try
        {
            var preferences = new UserPreference(request.LikesCommercial, request.Food,
                request.Culture, request.Entertainment, request.PlaceTypes);

            user.SetPreferences(preferences);

            userRepository.Update(user);
            await userPreferenceRepository.AddOrUpdateAsync(user.Preferences!);

            var groupMemberships = await groupMemberRepository.GetAllByUserIdAsync(user.Id);

            foreach (var membership in groupMemberships)
            {
                var group =
                    await groupRepository.GetGroupWithMembersPreferencesAsync(membership.GroupId);

                if (group is null)
                {
                    return BadRequest(
                        new ErrorResponse("Some of the groups that user is member were not found."));
                }

                group.UpdatePreferences();

                groupRepository.Update(group);
                groupPreferenceRepository.Update(group.Preferences);
            }

            await unitOfWork.SaveAsync();
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
    }
}
