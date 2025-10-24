using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.User;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/users")]
public class AdminUserController(
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGroupMemberRepository groupMemberRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRoleRepository userRoleRepository,
    IRedisService redisService): ControllerBase
{
    [HttpGet]

    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (users, hits) = await userRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new GetAllUsersResponse
        {
            Data = users.Select(x => new GetAllUsersResponseData { Id = x.Id, CreatedAt = x.CreatedAt }),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        return Ok(new GetUserByIdResponse
        {
            Name = user.Name,
            Email = user.Email,
            Preferences = new GetUserByIdPreferenceResponse { Categories = user.Preferences.Categories },
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email);

        if (existsUserWithEmail)
        {
            return Conflict(new ErrorResponse("There is already an user using this email."));
        }
        var defaultRole = await roleRepository.GetDefaultUserRoleAsync();
        if (defaultRole is null)
        {
            return NotFound(new ErrorResponse("Role not found."));
        }
        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(request.Name, email, passwordHash, defaultRole);

        await userRepository.AddAsync(user);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Create), new CreateUserResponse { Id = user.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUserById([FromRoute] Guid id, 
        [FromBody] UpdateUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserById([FromRoute] Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", user.Id.ToString());
        await redisService.DeleteAsync(key);

        userRepository.Remove(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/anonymize")]
    public async Task<IActionResult> AnonymizeUserById([FromRoute] Guid id)
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(id);

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

    [HttpPut("{id:guid}/preferences")]
    public async Task<IActionResult> SetUserPreferencesByUserId([FromRoute] Guid id, 
        [FromBody] SetPreferencesByUserIdRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        userRepository.Update(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}
