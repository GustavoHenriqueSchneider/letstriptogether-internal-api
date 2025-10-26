using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.GroupDestinationVote;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMemberDestinationVote;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/destination-votes")]
public class GroupDestinationVoteController(
    IApplicationUserContext currentUser,
    IGroupRepository groupRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IUserRepository userRepository,
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> VoteAtDestinationForGroupId([FromRoute] Guid groupId,
        [FromBody] VoteAtDestinationForGroupIdRequest request)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);

        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var destinationExists = await destinationRepository.ExistsByIdAsync(request.DestinationId);

        if (!destinationExists)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        var existsVote = 
            await groupMemberDestinationVoteRepository.ExistsByGroupMemberDestinationVoteByIdsAsync(
                groupMember.Id, request.DestinationId);
        
        if (existsVote)
        {
            return Conflict(new ErrorResponse("Vote already exists for the informed group and destination ids."));
        }

        var vote = new GroupMemberDestinationVote(groupMember.Id, request.DestinationId, request.IsApproved);

        await groupMemberDestinationVoteRepository.AddAsync(vote);
        await unitOfWork.SaveAsync();

        return Ok(new VoteAtDestinationForGroupIdResponse { Id = vote.Id });
    }

    [HttpPut("{destinationVoteId:guid}")]
    public async Task<IActionResult> UpdateDestinationVoteById([FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, [FromBody] UpdateDestinationVoteByIdRequest request)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);

        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdAsync(destinationVoteId);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Vote not found."));
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            return BadRequest(new ErrorResponse("You are not a owner of this vote."));
        }

        vote.SetApproved(request.IsApproved);
        groupMemberDestinationVoteRepository.Update(vote);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}

