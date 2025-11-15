using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.Invitation.Command.AcceptInvitation;
using Application.UseCases.v1.Invitation.Command.RefuseInvitation;
using Application.UseCases.v1.Invitation.Query.GetInvitationDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/invitations")]
public class InvitationController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Consultar Convite",
        Description = "Retorna informações básicas de um convite de grupo a partir do token informado.")]
    [ProducesResponseType(typeof(GetInvitationDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitationDetails([FromQuery] string token, CancellationToken cancellationToken)
    {
        var query = new GetInvitationDetailsQuery
        {
            Token = token
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost("accept")]
    [SwaggerOperation(
        Summary = "Aceitar Convite",
        Description = "Aceita um convite de grupo, adicionando o usuário autenticado como membro do grupo.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("refuse")]
    [SwaggerOperation(
        Summary = "Recusar Convite",
        Description = "Recusa um convite de grupo, marcando o convite como recusado.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefuseInvitation([FromBody] RefuseInvitationCommand command, CancellationToken cancellationToken)
    {
        command = command with
        {
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
