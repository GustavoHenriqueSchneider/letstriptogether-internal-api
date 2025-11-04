using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.GroupDestinationVote;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMemberDestinationVote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/destination-votes")]
public class GroupDestinationVoteController(
    IApplicationUserContext currentUser,
    IGroupRepository groupRepository,
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IUserRepository userRepository,
    IDestinationRepository destinationRepository,
    IGroupMatchRepository groupMatchRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> VoteAtDestinationForGroupId([FromRoute] Guid groupId,
        [FromBody] VoteAtDestinationForGroupIdRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);
        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var existsGroup = await groupRepository.ExistsByIdAsync(groupId, cancellationToken);
        if (!existsGroup)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var destinationExists = await destinationRepository.ExistsByIdAsync(request.DestinationId, cancellationToken);

        if (!destinationExists)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        var existsVote = 
            await groupMemberDestinationVoteRepository.ExistsByGroupMemberDestinationVoteByIdsAsync(
                groupMember.Id, request.DestinationId, cancellationToken);
        
        if (existsVote)
        {
            return Conflict(new ErrorResponse("Vote already exists for the informed group and destination ids."));
        }

        var vote = new GroupMemberDestinationVote(groupMember.Id, request.DestinationId, request.IsApproved);
        await groupMemberDestinationVoteRepository.AddAsync(vote, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        if (!vote.IsApproved)
        {
            return Ok(new VoteAtDestinationForGroupIdResponse { Id = vote.Id });
        }

        var group = await groupRepository.GetGroupWithMembersVotesAndMatchesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }
        
        try
        {
            var match = group.CreateMatch(request.DestinationId);
            await groupMatchRepository.AddAsync(match, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
                
            // TODO: criar service de notificação
        }
        catch
        {
            // purposed ignored
        }

        return Ok(new VoteAtDestinationForGroupIdResponse { Id = vote.Id });
    }

    [HttpPut("{destinationVoteId:guid}")]
    public async Task<IActionResult> UpdateDestinationVoteById([FromRoute] Guid groupId, 
        [FromRoute] Guid destinationVoteId, [FromBody] UpdateDestinationVoteByIdRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);
        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var existsGroup = await groupRepository.ExistsByIdAsync(groupId, cancellationToken);
        if (!existsGroup)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdAsync(destinationVoteId, cancellationToken);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Vote not found."));
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            return BadRequest(new ErrorResponse("You are not a owner of this vote."));
        }
        
        var match = await groupMatchRepository.GetByGroupAndDestinationAsync(groupId, vote.DestinationId, cancellationToken);
        if (match is not null)
        {
            return BadRequest(new ErrorResponse("There is already a match with this vote, you can not change it."));
        }

        vote.SetApproved(request.IsApproved);
        
        groupMemberDestinationVoteRepository.Update(vote);
        await unitOfWork.SaveAsync(cancellationToken);

        if (!vote.IsApproved)
        {
            return NoContent();
        }
        
        var group = await groupRepository.GetGroupWithMembersVotesAndMatchesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }
            
        try
        {
            match = group.CreateMatch(vote.DestinationId);
            await groupMatchRepository.AddAsync(match, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
            
            // TODO: criar service de notificação
        }
        catch
        {
            // purposed ignored
        }
        
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetGroupMemberAllDestinationVotesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);

        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByMemberIdAsync(groupMember.Id,
            pageNumber, pageSize, cancellationToken);

        return Ok(new GetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new GetGroupMemberAllDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{destinationVoteId:guid}")]
    public async Task<IActionResult> GetGroupDestinationVoteById(
        [FromRoute] Guid groupId, [FromRoute] Guid destinationVoteId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == groupId);

        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var vote = await groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(groupId,
            destinationVoteId, cancellationToken);

        if (vote is null)
        {
            return NotFound(new ErrorResponse("Group member destination vote not found."));
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            return BadRequest(new ErrorResponse("You are not a owner of this vote."));
        }

        return Ok(new GetGroupDestinationVoteByIdResponse
        {
            DestinationId = vote.DestinationId,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        });
    }
}
