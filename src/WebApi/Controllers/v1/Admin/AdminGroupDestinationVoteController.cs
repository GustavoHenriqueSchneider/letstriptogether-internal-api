using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;
using LetsTripTogether.InternalApi.Application.UseCases.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/destination-votes")]
public class AdminGroupDestinationVoteController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupDestinationVotesById(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllGroupDestinationVotesByIdQuery
        {
            GroupId = groupId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{destinationVoteId:guid}")]
    public async Task<IActionResult> AdminGetGroupDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid destinationVoteId, CancellationToken cancellationToken)
    {
        var query = new AdminGetGroupDestinationVoteByIdQuery
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
