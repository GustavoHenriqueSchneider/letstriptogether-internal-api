using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/matches")]
public class GroupMatchController(
    IMediator mediator,
    IApplicationUserContextExtensions currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetAllGroupMatchesByIdQuery
        {
            GroupId = groupId,
            UserId = currentUser.GetId(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{matchId:guid}")]
    public async Task<IActionResult> GetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var query = new GetGroupMatchByIdQuery
        {
            GroupId = groupId,
            MatchId = matchId,
            UserId = currentUser.GetId()
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<IActionResult> RemoveGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var command = new RemoveGroupMatchByIdCommand
        {
            GroupId = groupId,
            MatchId = matchId,
            UserId = currentUser.GetId()
        };

        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
