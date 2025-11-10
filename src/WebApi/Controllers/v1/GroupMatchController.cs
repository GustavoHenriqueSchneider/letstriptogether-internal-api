using Application.Common.Interfaces.Extensions;
using Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;
using Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;
using Application.UseCases.GroupMatch.Query.GetGroupMatchById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

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
    [SwaggerOperation(
        Summary = "Remover Match do Grupo",
        Description = "Remove um match do grupo. Apenas o proprietário do grupo pode remover matches.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveGroupMatchById(
        [FromRoute] Guid groupId,
        [FromRoute] Guid matchId, 
        CancellationToken cancellationToken)
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
