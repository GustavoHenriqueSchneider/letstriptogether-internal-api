using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/matches")]
public class AdminGroupMatchController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupMatchesByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{matchId:guid}")]
    public async Task<IActionResult> AdminGetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupMatchByIdQuery
        {
            GroupId = groupId,
            MatchId = matchId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
