using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMemberDestinationVote;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/admin/group-member-destination-votes")]
public class AdminGroupMemberDestinationVoteController(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (votes, hits) = await groupMemberDestinationVoteRepository.GetAllWithRelationsAsync(pageNumber, pageSize);

        return Ok(new GetAllGroupMemberDestinationVotesResponse
        {
            Data = votes.Select(MapToResponseData),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(id);

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

    [HttpGet("by-group/{groupId:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetByGroupId([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetAllGroupMemberDestinationVotesResponse
        {
            Data = votes.Select(MapToResponseData),
            Hits = hits
        });
    }

    private static GetAllGroupMemberDestinationVotesResponseData MapToResponseData(Models.GroupMemberDestinationVote vote)
    {
        return new GetAllGroupMemberDestinationVotesResponseData
        {
            Id = vote.Id,
            GroupMemberId = vote.GroupMemberId,
            GroupMemberName = vote.GroupMember.User.Name,
            DestinationId = vote.DestinationId,
            DestinationAddress = vote.Destination.Address,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt
        };
    }
}
