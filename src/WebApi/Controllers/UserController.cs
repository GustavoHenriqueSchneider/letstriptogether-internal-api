using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
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

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UserController(
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGroupMemberRepository groupMemberRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRoleRepository userRoleRepository): ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]

    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
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
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
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
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
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
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> UpdateById([FromRoute] Guid id, 
        [FromBody] UpdateUserRequest request)
    {
        // TODO: converter updaterequest em record pra adicionar
        var user = await userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}

        userRepository.Remove(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/anonymize")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> AnonymizeById([FromRoute] Guid id)
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

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("{id:guid}/preferences")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> SetPreferencesByUserId([FromRoute] Guid id, 
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

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

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

    [HttpPut("me")]
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

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {

        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        userRepository.Remove(user);
        await unitOfWork.SaveAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}

        return NoContent();
    }

    [HttpPatch("me/anonymize")]
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

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("me/preferences")]
    public async Task<IActionResult> SetCurrentUserPreferences(
        [FromBody] SetCurrentUserPreferencesRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

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
