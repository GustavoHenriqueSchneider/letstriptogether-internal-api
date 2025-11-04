using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.Helpers;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Admin.User;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/users")]
public class AdminUserController(
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGroupMemberRepository groupMemberRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRoleRepository userRoleRepository,
    IUserPreferenceRepository userPreferenceRepository,
    IGroupRepository groupRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IRedisService redisService): ControllerBase
{
    [HttpGet]

    public async Task<IActionResult> AdminGetAllUsers([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var (users, hits) = await userRepository.GetAllAsync(pageNumber, pageSize, cancellationToken);

        return Ok(new AdminGetAllUsersResponse
        {
            Data = users.Select(x => new AdminGetAllUsersResponseData { Id = x.Id, CreatedAt = x.CreatedAt }),
            Hits = hits
        });
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> AdminGetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        try
        {
            _ = new UserPreference(user.Preferences);

            return Ok(new AdminGetUserByIdResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = user.Preferences is not null ?
                    new AdminGetUserByIdPreferenceResponse
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
                new ErrorResponse("Invalid preferences are filled for this user, please fix it."));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }
        var defaultRole = await roleRepository.GetDefaultUserRoleAsync(cancellationToken);
        if (defaultRole is null)
        {
            return NotFound(new ErrorResponse("Role not found."));
        }
        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(request.Name, email, passwordHash, defaultRole);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return CreatedAtAction(nameof(AdminCreateUser), new AdminCreateUserResponse { Id = user.Id });
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> AdminUpdateUserById([FromRoute] Guid userId, 
        [FromBody] AdminUpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> AdminDeleteUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = KeyHelper.UserRefreshToken(user.Id);
        await redisService.DeleteAsync(key);

        // TODO: parou de funcionar
        userRepository.Remove(user);
        await unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

    [HttpPatch("{userId:guid}/anonymize")]
    public async Task<IActionResult> AdminAnonymizeUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(userId, cancellationToken);

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

    [HttpPut("{userId:guid}/preferences")]
    public async Task<IActionResult> AdminSetUserPreferencesByUserId([FromRoute] Guid userId, 
        [FromBody] AdminSetUserPreferencesByUserIdRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(userId, cancellationToken);

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
