using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/invitations")]
public class InvitationController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new AcceptInvitationCommand
        {
            Token = command.Token,
            UserId = currentUser.GetId()
        };

        await mediator.Send(commandWithContext, cancellationToken);
        return Ok();
    }

    [HttpPost("refuse")]
    public async Task<IActionResult> RefuseInvitation([FromBody] RefuseInvitationCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new RefuseInvitationCommand
        {
            Token = command.Token,
            UserId = currentUser.GetId()
        };

        await mediator.Send(commandWithContext, cancellationToken);
        return Ok();
    }
}
