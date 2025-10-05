// WebApi.Controllers/UsersController.cs (COMPLETAMENTE REFATORADO)

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context;
using WebApi.DTOs.Requests.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.User;
using WebApi.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/users")]
public class UsersController(
    // Injeção: Usando o Gerente (UnitOfWork)
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IApplicationUserContext currentUser)
    : ControllerBase
{
    // Rota: GET /users (ADMIN)
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAll()
    {
        // Usa o IBaseRepository.GetAllAsync (do UserRepository)
        var users = await unitOfWork.Users.GetAllAsync(0, 100);

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

    // Rota: GET /users/{id} (ADMIN)
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        // Usa o IUserRepository.GetByIdWithPreferencesAsync
        var user = await unitOfWork.Users.GetByIdWithPreferencesAsync(id);

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

    // Rota: POST /users (ADMIN) - Criação com checagem de e-mail e Role
    [HttpPost]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateRequest request)
    {
        var email = request.Email;

        // Usa IUserRepository.ExistsByEmailAsync (Checagem otimizada)
        var existsUserWithEmail = await unitOfWork.Users.ExistsByEmailAsync(email);

        if (existsUserWithEmail)
        {
            return Conflict(new BaseResponse { Status = "Error", Message = "There is already an user using this email." });
        }

        // Usa IUserRepository.GetDefaultUserRoleAsync (Busca de Role)
        var defaultRole = await unitOfWork.Users.GetDefaultUserRoleAsync();

        if (defaultRole is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "Role not found." });
        }

        var passwordHash = passwordHashService.HashPassword(request.Password);
        var user = new User(request.Name, email, passwordHash, defaultRole);

        // Usa IBaseRepository.AddAsync e UnitOfWork.SaveAsync
        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(Create), new BaseResponse
        {
            Status = "Success",
            Message = "User was created.",
            Data = new CreateResponse { Id = user.Id }
        });
    }

    // Rota: PUT /users/{id} (ADMIN)
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> UpdateById([FromRoute] Guid id, [FromBody] UpdateRequest request)
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        user.Update(request.Name);

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: DELETE /users/{id} (ADMIN)
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id)
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        // Usa IBaseRepository.Remove e UnitOfWork.SaveAsync
        unitOfWork.Users.Remove(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: PATCH /users/{id}/anonymize (ADMIN)
    [HttpPatch("{id:guid}/anonymize")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> AnonymizeById([FromRoute] Guid id)
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        user.Anonymize();

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: PUT /users/{id}/preferences (ADMIN)
    [HttpPut("{id:guid}/preferences")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> SetPreferencesById([FromRoute] Guid id, [FromBody] SetPreferencesRequest request)
    {
        // Usa IUserRepository.GetByIdWithPreferencesAsync
        var user = await unitOfWork.Users.GetByIdWithPreferencesAsync(id);

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: GET /users/me
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Usa IUserRepository.GetByIdWithPreferencesAsync com o ID do usuário logado
        var user = await unitOfWork.Users.GetByIdWithPreferencesAsync(currentUser.GetId());

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

    // Rota: PUT /users/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateForCurrentUserRequest request)
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        user.Update(request.Name);

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: DELETE /users/me
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        // Usa IBaseRepository.Remove e UnitOfWork.SaveAsync
        unitOfWork.Users.Remove(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: PATCH /users/me/anonymize
    [HttpPatch("me/anonymize")]
    public async Task<IActionResult> AnonymizeCurrentUser()
    {
        // Usa IBaseRepository.GetByIdAsync
        var user = await unitOfWork.Users.GetByIdAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        user.Anonymize();

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    // Rota: PUT /users/me/preferences
    [HttpPut("me/preferences")]
    public async Task<IActionResult> SetPreferencesForCurrentUser(
        [FromBody] SetPreferencesForCurrentUserRequest request)
    {
        // Usa IUserRepository.GetByIdWithPreferencesAsync
        var user = await unitOfWork.Users.GetByIdWithPreferencesAsync(currentUser.GetId());

        if (user is null)
        {
            return NotFound(new BaseResponse { Status = "Error", Message = "User not found." });
        }

        var preferences = new UserPreference { Categories = request.Categories };
        user.SetPreferences(preferences);

        // Salva as mudanças
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}