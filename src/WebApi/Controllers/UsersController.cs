using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using WebApi.Context;
=======
using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Context.Interfaces;
>>>>>>> master
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.User;
using WebApi.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
<<<<<<< HEAD
using WebApi.Services;
using System.Linq;
=======
using WebApi.Services.Interfaces;
>>>>>>> master

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr, repository, DI e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: definir retorno das rotas com classes de response e converter returns de erro em exception

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UsersController(
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IRoleRepository roleRepository
    )
    : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]

    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, int pageSize = 10)
    {
        var users = await userRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "List of users returned.",
            Data = new GetAllResponse
            {
                Users = users.Select(x => new GetAllUsersResponse { Id = x.Id, CreatedAt = x.CreatedAt }),
                Hits = users.Count()
            }
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "User found.",
            Data = new GetByIdResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = new GetByIdUserPreferenceResponse { Categories = user.Preferences.Categories },
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        });
    }

    [HttpPost]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var email = request.Email;
        var existsUserWithEmail = await userRepository.ExistsByEmailAsync(email);

        if (existsUserWithEmail)
        {
            return Conflict(new BaseResponse { Status = "Error", Message = "There is already an user using this email." });
        }
        var defaultRole = await roleRepository.GetDefaultUserRoleAsync();
        if (defaultRole is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "Role not found." });
        }
        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(request.Name, email, passwordHash, defaultRole);

        await userRepository.AddAsync(user);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Create), new BaseResponse
        {
            Status = "Success",
            Message = "User was created.",
            Data = new CreateResponse { Id = user.Id }
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> UpdateById([FromRoute] Guid id, [FromBody] UpdateRequest request)
    {
        // TODO: converter updaterequest em record pra adicionar
        var user = await userRepository.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
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
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
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
<<<<<<< HEAD
        var user = await userRepository.GetByIdAsync(id);
=======
        var user = await context.Users
            .Include(u => u.GroupMemberships)
            .Include(u => u.AcceptedInvitations)
            .Include(u => u.Preferences)
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(x => x.Id == id);
>>>>>>> master

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        context.GroupMembers.RemoveRange(user.GroupMemberships);
        context.UserGroupInvitations.RemoveRange(user.AcceptedInvitations);
        context.UserRoles.RemoveRange(user.UserRoles);

        user.Anonymize();
<<<<<<< HEAD

        await unitOfWork.SaveAsync();
=======
        await context.SaveChangesAsync();
>>>>>>> master

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("{id:guid}/preferences")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> SetPreferencesById([FromRoute] Guid id, [FromBody] SetPreferencesRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "User found.",
            Data = new GetByIdResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = new GetByIdUserPreferenceResponse { Categories = user.Preferences.Categories },
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        });
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateForCurrentUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        user.Update(request.Name);

        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {

        var user = await userRepository.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        userRepository.Remove(user);
        await unitOfWork.SaveAsync();
        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}git a

        return NoContent();
    }

    [HttpPatch("me/anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser()
    {
<<<<<<< HEAD
        var user = await userRepository.GetByIdAsync(currentUser.GetId());
=======
        var user = await context.Users
            .Include(u => u.GroupMemberships)
            .Include(u => u.AcceptedInvitations)
            .Include(u => u.Preferences)
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(x => x.Id == currentUser.GetId());
>>>>>>> master

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        context.GroupMembers.RemoveRange(user.GroupMemberships);
        context.UserGroupInvitations.RemoveRange(user.AcceptedInvitations);
        context.UserRoles.RemoveRange(user.UserRoles);

        user.Anonymize();
<<<<<<< HEAD
        // TODO: remover vinculos de usuario com grupos...
        await unitOfWork.SaveAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
=======
        await context.SaveChangesAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

>>>>>>> master
        return NoContent();
    }

    [HttpPut("me/preferences")]
    public async Task<IActionResult> SetPreferencesForCurrentUser(
        [FromBody] SetPreferencesForCurrentUserRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        await unitOfWork.SaveAsync();

        return NoContent();
    }
}