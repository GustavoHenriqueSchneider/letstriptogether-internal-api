using LetsTripTogether.InternalApi.Application.Common.Policies;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupMatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/groups/{groupId:guid}/matches")]
public class AdminGroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId, cancellationToken);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMatches, hits) = 
            await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize, cancellationToken);

        return Ok(new AdminGetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new AdminGetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{matchId:guid}")]
    public async Task<IActionResult> AdminGetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMatchesAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMatch = group.Matches.SingleOrDefault(x => x.Id == matchId);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        return Ok(new AdminGetGroupMatchByIdResponse
        {
            DestinationId = groupMatch.DestinationId,
            CreatedAt = groupMatch.CreatedAt,
            UpdatedAt = groupMatch.UpdatedAt
        });
    }
}
