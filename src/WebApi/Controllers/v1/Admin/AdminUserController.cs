using Application.Common.Policies;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminAnonymizeUserById;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminCreateUser;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminDeleteUserById;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using Application.UseCases.v1.Admin.AdminUser.Command.AdminUpdateUserById;
using Application.UseCases.v1.Admin.AdminUser.Query.AdminGetAllUsers;
using Application.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/users")]
public class AdminUserController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Usuários (Admin)",
        Description = "Retorna uma lista paginada de todos os usuários do sistema. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetAllUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminGetAllUsers(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{userId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Usuário por ID (Admin)",
        Description = "Retorna os detalhes de um usuário específico, incluindo preferências. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetUserByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminGetUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var query = new AdminGetUserByIdQuery
        {
            UserId = userId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Criar Usuário (Admin)",
        Description = "Cria um novo usuário no sistema. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminCreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(AdminCreateUser), response);
    }

    [HttpPut("{userId:guid}")]
    [SwaggerOperation(
        Summary = "Atualizar Usuário por ID (Admin)",
        Description = "Atualiza as informações de um usuário específico. Requer permissões de administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminUpdateUserById([FromRoute] Guid userId, 
        [FromBody] AdminUpdateUserByIdCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    [SwaggerOperation(
        Summary = "Excluir Usuário por ID (Admin)",
        Description = "Exclui permanentemente um usuário do sistema. Requer permissões de administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminDeleteUserById(
        [FromRoute] Guid userId, 
        CancellationToken cancellationToken)
    {
        var command = new AdminDeleteUserByIdCommand
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{userId:guid}/anonymize")]
    [SwaggerOperation(
        Summary = "Anonimizar Usuário por ID (Admin)",
        Description = "Anonimiza os dados pessoais de um usuário específico, mantendo a conta ativa mas removendo informações identificáveis. Requer permissões de administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminAnonymizeUserById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new AdminAnonymizeUserByIdCommand
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("{userId:guid}/preferences")]
    [SwaggerOperation(
        Summary = "Definir Preferências do Usuário por ID (Admin)",
        Description = "Define ou atualiza as preferências de viagem de um usuário específico. Requer permissões de administrador.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdminSetUserPreferencesByUserId(
        [FromRoute] Guid userId, 
        [FromBody] AdminSetUserPreferencesByUserIdCommand command, 
        CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = userId
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
