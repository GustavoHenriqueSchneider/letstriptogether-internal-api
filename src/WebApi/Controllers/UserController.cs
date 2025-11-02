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

[ApiController]
[Authorize]
[Route("api/v1/users/me")]
[Tags("Usuário")]
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
    /// <summary>
    ///  Busca o usuário atual.
    /// </summary>
    /// <remarks>
    /// Retorna os dados do usuário autenticado incluindo preferências.
    /// </remarks>
    /// <response code="200">Retorna os dados do usuário atual</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetCurrentUserResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <summary>
    /// Atualiza o usuário atual.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="204">Usuário atualizado com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <summary>
    /// Deleta o usuário atual.
    /// </summary>
    /// <response code="204">Usuário deletado com sucesso</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <summary>
    /// Anonimiza o usuário atual.
    /// </summary>
    /// <response code="204">Usuário anonimizado com sucesso</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPatch("anonymize")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <summary>
    /// Define as preferências do usuário atual.
    /// </summary>
    /// <param name="request"></param>
    /// <response code="204">Preferências definidas com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpPut("preferences")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
