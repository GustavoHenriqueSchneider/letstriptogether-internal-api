using Application.Common.Interfaces.Extensions;
using Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;
using Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;
using Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/invitations")]
public class GroupInvitationController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Criar Convite de Grupo",
        Description = "Cria um novo convite para o grupo especificado. Apenas o proprietário do grupo pode criar convites.")]
    [ProducesResponseType(typeof(CreateGroupInvitationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGroupInvitation(
        [FromRoute] Guid groupId, 
        CancellationToken cancellationToken)
    {
        var command = new CreateGroupInvitationCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CreateGroupInvitation), response);
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Obter Convite Ativo do Grupo",
        Description = "Retorna o convite ativo do grupo, se existir. Apenas o proprietário do grupo pode visualizar.")]
    [ProducesResponseType(typeof(GetActiveGroupInvitationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetActiveGroupInvitationQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("cancel")]
    [SwaggerOperation(
        Summary = "Cancelar Convite Ativo do Grupo",
        Description = "Cancela o convite ativo do grupo especificado. Apenas o proprietário do grupo pode cancelar convites.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelActiveGroupInvitation(
        [FromRoute] Guid groupId, 
        CancellationToken cancellationToken)
    {
        var command = new CancelActiveGroupInvitationCommand
        {
            GroupId = groupId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
