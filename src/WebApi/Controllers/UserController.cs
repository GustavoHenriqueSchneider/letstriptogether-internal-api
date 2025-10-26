using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.User;
using WebApi.Models;
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

        return Ok(new GetCurrentUserResponse
        {
            Name = user.Name,
            Email = user.Email,
            Preferences = new GetCurrentUserPreferenceResponse { Categories = user.Preferences.Categories.ToList() },
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
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

        var preferences = new UserPreference(request.Categories.ToList());
        user.SetPreferences(preferences);

        // TODO: ajustar logica do update para atualizar entidades filhas/relacionadas
        userRepository.Update(user);
        userPreferenceRepository.Update(user.Preferences);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}
