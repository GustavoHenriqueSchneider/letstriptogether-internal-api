using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/destination-votes")]
public class GroupDestinationVoteController(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> VoteAtDestinationForGroupId([FromBody] CreateVoteRequest request)
    {
        var existingVote = await groupMemberDestinationVoteRepository.GetByGroupMemberAndDestinationAsync(
            request.GroupMemberId, request.DestinationId);
        
        if (existingVote is not null)
        {
            return Conflict(new ErrorResponse("Vote already exists."));
        }

        var vote = new GroupMemberDestinationVote(
            request.GroupMemberId, 
            request.DestinationId, 
            request.IsApproved);

        await groupMemberDestinationVoteRepository.AddAsync(vote);
        await unitOfWork.SaveAsync();

        return Ok(new { id = vote.Id });
    }

    [HttpPut("{destinationVoteId:guid}")]
    public async Task<IActionResult> UpdateDestinationVoteById([FromRoute] Guid destinationVoteId, 
        [FromBody] UpdateVoteRequest request)
    {
        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(destinationVoteId);
        if (vote is null)
        {
            return NotFound(new ErrorResponse("Vote not found."));
        }

        vote.Update(request.IsApproved);
        groupMemberDestinationVoteRepository.Update(vote);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}

public class CreateVoteRequest
{
    public Guid GroupMemberId { get; set; }
    public Guid DestinationId { get; set; }
    public bool IsApproved { get; set; }
}

public class UpdateVoteRequest
{
    public bool IsApproved { get; set; }
}