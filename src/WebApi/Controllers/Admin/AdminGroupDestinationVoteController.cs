using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMemberDestinationVote;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/destination-votes")]
public class AdminGroupDestinationVoteController(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository) : ControllerBase
{
    public async Task<IActionResult> AdminGetAllGroupDestinationVotesById(
        [FromRoute] Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (votes, hits) = 
            await groupMemberDestinationVoteRepository.GetByGroupIdAsync(groupId, 
                pageNumber, pageSize);

        return Ok(new AdminGetAllGroupDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new AdminGetAllGroupDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{destinationVoteId:guid}")]
    public async Task<IActionResult> AdminGetGroupDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid destinationVoteId)
    {
        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(groupId, 
            destinationVoteId);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Group member destination vote not found."));
        }

        return Ok(new AdminGetGroupDestinationVoteByIdResponse
        {
            GroupId = vote.GroupMember.GroupId,
            MemberId = vote.GroupMemberId,
            DestinationId = vote.DestinationId,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        });
    }
}
