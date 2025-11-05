using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/destination-votes")]
public class GroupDestinationVoteController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> VoteAtDestinationForGroupId([FromRoute] Guid groupId,
        [FromBody] VoteAtDestinationForGroupIdCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new VoteAtDestinationForGroupIdCommand
        {
            GroupId = groupId,
            DestinationId = command.DestinationId,
            IsApproved = command.IsApproved,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(commandWithContext, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{destinationVoteId:guid}")]
    public async Task<IActionResult> UpdateDestinationVoteById([FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, [FromBody] UpdateDestinationVoteByIdCommand command, CancellationToken cancellationToken)
    {
        var commandWithContext = new UpdateDestinationVoteByIdCommand
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId,
            IsApproved = command.IsApproved,
            UserId = currentUser.GetId()
        };

        await mediator.Send(commandWithContext, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetGroupMemberAllDestinationVotesByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{destinationVoteId:guid}")]
    public async Task<IActionResult> GetGroupDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid destinationVoteId, CancellationToken cancellationToken)
    {
        var query = new GetGroupDestinationVoteByIdQuery
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
