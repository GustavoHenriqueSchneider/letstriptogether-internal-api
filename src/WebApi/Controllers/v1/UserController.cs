using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.User;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/users/me")]
public class UserController(
    IUnitOfWork unitOfWork,
    IApplicationUserContextExtensions currentUser,
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
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId(), cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        try
        {
            _ = new UserPreference(user.Preferences);

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
        [FromBody] UpdateCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId(), cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId(), cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        // TODO: parou de funcionar
        userRepository.Remove(user);
        await unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);

        return NoContent();
    }

    [HttpPatch("anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser(CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(currentUser.GetId(), cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        groupMemberRepository.RemoveRange(user.GroupMemberships);
        userGroupInvitationRepository.RemoveRange(user.AcceptedInvitations);
        userRoleRepository.RemoveRange(user.UserRoles);

        user.Anonymize();

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);

        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> SetCurrentUserPreferences(
        [FromBody] SetCurrentUserPreferencesRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId(), cancellationToken);

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
            await userPreferenceRepository.AddOrUpdateAsync(user.Preferences!, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            var groupMemberships = 
                (await groupMemberRepository.GetAllByUserIdAsync(user.Id, cancellationToken)).ToList();

            foreach (var membership in groupMemberships)
            {
                var group =
                    await groupRepository.GetGroupWithMembersPreferencesAsync(membership.GroupId, cancellationToken);

                if (group is null)
                {
                    return BadRequest(
                        new ErrorResponse("Some of the groups that user is member were not found."));
                }
                
                group.UpdatePreferences();
                var groupToUpdate = await groupRepository.GetGroupWithPreferencesAsync(membership.GroupId, cancellationToken);

                if (groupToUpdate is null)
                {
                    return BadRequest(
                        new ErrorResponse("Some of the groups that user is member were not found in the database."));
                }
                
                groupToUpdate.Preferences.Update(group.Preferences);

                groupRepository.Update(groupToUpdate);
                groupPreferenceRepository.Update(groupToUpdate.Preferences);
            }

            if (groupMemberships.Count != 0)
            {
                await unitOfWork.SaveAsync(cancellationToken);
            }
            
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Invalid"))
        {
            return UnprocessableEntity(new ErrorResponse(ex.Message));
        }
    }
}
