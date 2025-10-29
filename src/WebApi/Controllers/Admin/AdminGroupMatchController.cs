using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMatch;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/matches")]
public class AdminGroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMatches, hits) = 
            await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

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
        [FromRoute] Guid matchId)
    {
        var group = await groupRepository.GetGroupWithMatchesAsync(groupId);

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
