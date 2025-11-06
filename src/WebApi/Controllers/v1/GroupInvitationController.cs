using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/invitations")]
public class GroupInvitationController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
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
    public async Task<IActionResult> CancelActiveGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
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
