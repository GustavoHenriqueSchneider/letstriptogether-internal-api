using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.User;
using WebApi.Models;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr, repository, DI e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: definir retorno das rotas com classes de response e converter returns de erro em exception
[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UsersController(AppDbContext context, IPasswordHashService passwordHashService,
    IApplicationUserContext currentUser)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    // TODO: obter paginaçao de query params
    public async Task<IActionResult> GetAll()
    {
        var users = await context.Users.AsNoTracking().ToListAsync();
        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "List of users returned.",
            Data = new GetAllResponse
            {
                Users = users.Select(x => new GetAllUsersResponse { Id = x.Id, CreatedAt = x.CreatedAt }),
                Hits = users.Count
            }
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var user = await context.Users
            .Include(x => x.Preferences)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "User found.",
            Data = new GetByIdResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = new GetByIdUserPreferenceResponse
                {
                    Categories = user.Preferences.Categories
                },
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
        var existsUserWithEmail = await context.Users.AsNoTracking().AnyAsync(x => x.Email == email);

        if (existsUserWithEmail)
        {
            return Conflict(new BaseResponse
            {
                Status = "Error",
                Message = "There is already an user using this email."
            });
        }

        var defaultRole = await context.Roles.SingleOrDefaultAsync(x => x.Name == Roles.User);

        if (defaultRole is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "Role not found."
            });
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(request.Name, email, passwordHash, defaultRole);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

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
        // TODO: converter updaterequest em record pra adicionar id nele
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        user.Update(request.Name);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}

        return NoContent();
    }

    [HttpPatch("{id:guid}/anonymize")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> AnonymizeById([FromRoute] Guid id)
    {
        var user = await context.Users
            .Include(u => u.GroupMemberships)
            .Include(u => u.AcceptedInvitations)
            .Include(u => u.Preferences)
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        context.GroupMembers.RemoveRange(user.GroupMemberships);
        context.UserGroupInvitations.RemoveRange(user.AcceptedInvitations);
        context.UserRoles.RemoveRange(user.UserRoles);

        user.Anonymize();
        await context.SaveChangesAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("{id:guid}/preferences")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> SetPreferencesById([FromRoute] Guid id,
        [FromBody] SetPreferencesRequest request)
    {
        var user = await context.Users
            .Include(x => x.Preferences)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await context.Users
            .Include(x => x.Preferences)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        return Ok(new BaseResponse
        {
            Status = "Success",
            Message = "User found.",
            Data = new GetByIdResponse
            {
                Name = user.Name,
                Email = user.Email,
                Preferences = new GetByIdUserPreferenceResponse
                {
                    Categories = user.Preferences.Categories
                },
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        });
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateForCurrentUserRequest request)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        user.Update(request.Name);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}

        return NoContent();
    }

    [HttpPatch("me/anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser()
    {
        var user = await context.Users
            .Include(u => u.GroupMemberships)
            .Include(u => u.AcceptedInvitations)
            .Include(u => u.Preferences)
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        context.GroupMembers.RemoveRange(user.GroupMemberships);
        context.UserGroupInvitations.RemoveRange(user.AcceptedInvitations);
        context.UserRoles.RemoveRange(user.UserRoles);

        user.Anonymize();
        await context.SaveChangesAsync();

        // TODO: remover refreshtoken no redis em auth:refresh_token:{userId}
        // TODO: registrar log de auditoria da anonimização do usuário
        // TODO: criar entrada na tabela DataDeletionAudit com motivo, timestamp e dados removidos

        return NoContent();
    }

    [HttpPut("me/preferences")]
    public async Task<IActionResult> SetPreferencesForCurrentUser(
        [FromBody] SetPreferencesForCurrentUserRequest request)
    {
        var user = await context.Users
            .Include(x => x.Preferences)
            .SingleOrDefaultAsync(x => x.Id == currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse
            {
                Status = "Error",
                Message = "User not found."
            });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        await context.SaveChangesAsync();
        return NoContent();
    }
}
