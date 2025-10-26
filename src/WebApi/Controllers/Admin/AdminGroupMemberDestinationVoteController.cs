using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMemberDestinationVote;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/groups/{groupId:guid}/members/{memberId:guid}/destination-votes")]
public class AdminGroupMemberDestinationVoteController(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllGroupMemberDestinationVotesById(
        [FromRoute] Guid groupId, [FromRoute] Guid memberId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (votes, hits) = await groupMemberDestinationVoteRepository.GetAllWithRelationsAsync(pageNumber, pageSize);

        return Ok(new GetAllGroupMemberDestinationVotesResponse
        {
            Data = votes.Select(MapToResponseData),
            Hits = hits
        });
    }

    [HttpGet("{destinationVoteId:guid}")]
    public async Task<IActionResult> GetGroupMemberDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid memberId, [FromRoute] Guid destinationVoteId)
    {
        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(destinationVoteId);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Group member destination vote not found."));
        }

        return Ok(new GetGroupMemberDestinationVoteByIdResponse
        {
            Id = vote.Id,
            GroupMemberId = vote.GroupMemberId,
            GroupMemberName = vote.GroupMember.User.Name,
            GroupMemberEmail = vote.GroupMember.User.Email,
            GroupId = vote.GroupMember.GroupId,
            GroupName = vote.GroupMember.Group.Name,
            DestinationId = vote.DestinationId,
            DestinationAddress = vote.Destination.Address,
            DestinationCategories = vote.Destination.Categories,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        });
    }
}
