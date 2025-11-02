using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Requests.Admin.User;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.User;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/users")]
[Tags("Admin - Usuários")]
public class AdminUserController(
    IUnitOfWork unitOfWork,
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IGroupMemberRepository groupMemberRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRoleRepository userRoleRepository,
    IUserPreferenceRepository userPreferenceRepository,
    IRedisService redisService): ControllerBase
{
    /// <summary>
    ///  Busca todos os usuários (Admin).
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os usuários ordenado por paginação.
    /// </remarks>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os usuários</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(AdminGetAllUsersResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdminGetAllUsers([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (users, hits) = await userRepository.GetAllAsync(pageNumber, pageSize);

        return Ok(new AdminGetAllUsersResponse
        {
            Data = users.Select(x => new AdminGetAllUsersResponseData { Id = x.Id, CreatedAt = x.CreatedAt }),
            Hits = hits
        });
    }
    /// <summary>
    /// Busca um usuário pelo Id (Admin).
    /// </summary>
    /// <param name="userId">Retorna o Guid do usuário a ser buscado</param>
    /// <response code="200">Retorna o usuário buscado pelo Id</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(AdminGetUserByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetUserById([FromRoute] Guid userId)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        return Ok(new AdminGetUserByIdResponse
        {
            Name = user.Name,
            Email = user.Email,
            Preferences = new AdminGetUserByIdPreferenceResponse { Categories = user.Preferences.Categories.ToList() },
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }
    /// <summary>
    /// Cria um novo usuário (Admin).
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Usuário criado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Role não encontrado</response>
    /// <response code="409">Já existe um usuário usando este email</response>
    [HttpPost]
    [ProducesResponseType(typeof(AdminCreateUserResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserRequest request)
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

        return CreatedAtAction(nameof(AdminCreateUser), new AdminCreateUserResponse { Id = user.Id });
    }
    /// <summary>
    /// Atualiza um usuário pelo Id (Admin).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <response code="204">Usuário atualizado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminUpdateUserById([FromRoute] Guid userId, 
        [FromBody] AdminUpdateUserRequest request)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        user.Update(request.Name);

        userRepository.Update(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
    /// <summary>
    /// Deleta um usuário pelo Id (Admin).
    /// </summary>
    /// <param name="userId"></param>
    /// <response code="204">Usuário deletado com sucesso</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeleteUserById([FromRoute] Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var key = RedisKeys.UserRefreshToken.Replace("{userId}", user.Id.ToString());
        await redisService.DeleteAsync(key);

        // TODO: parou de funcionar
        userRepository.Remove(user);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
    /// <summary>
    /// Anonimiza um usuário pelo Id (Admin).
    /// </summary>
    /// <param name="userId"></param>
    /// <response code="204">Usuário anonimizado com sucesso</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPatch("{userId:guid}/anonymize")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminAnonymizeUserById([FromRoute] Guid userId)
    {
        var user = await userRepository.GetUserWithRelationshipsByIdAsync(userId);

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
    /// <summary>
    /// Define as preferências de um usuário pelo Id (Admin).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <response code="204">Preferências definidas com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("{userId:guid}/preferences")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminSetUserPreferencesByUserId([FromRoute] Guid userId, 
        [FromBody] AdminSetUserPreferencesByUserIdRequest request)
    {
        var user = await userRepository.GetByIdWithPreferencesAsync(userId);

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
