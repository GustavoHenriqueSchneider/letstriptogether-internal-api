using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/matches")]
public class GroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IApplicationUserContextExtensions currentUser,
    IUserRepository userRepository,
    IGroupMemberDestinationVoteRepository groupDestinationVoteRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupExists = await groupRepository.ExistsByIdAsync(groupId, cancellationToken);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId, cancellationToken);

        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var (groupMatches, hits) =
            await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize, cancellationToken);

        return Ok(new GetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new GetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{matchId:guid}")]
    public async Task<IActionResult> GetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMatchesAsync(groupId, cancellationToken);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId, cancellationToken);

        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var groupMatch = group.Matches.SingleOrDefault(x => x.Id == matchId);

        if (groupMatch is null)
        {
            return NotFound(new ErrorResponse("Group match not found."));
        }

        return Ok(new GetGroupMatchByIdResponse
        {
            DestinationId = groupMatch.DestinationId,
            CreatedAt = groupMatch.CreatedAt,
            UpdatedAt = groupMatch.UpdatedAt
        });
    }

    [HttpDelete("{matchId:guid}")]
    public async Task<IActionResult> RemoveGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAndMatchesAsync(groupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.UserId == currentUserId);
        if (groupMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var matchToRemove = group.Matches.SingleOrDefault(m => m.Id == matchId);
        if (matchToRemove is null)
        {
            return NotFound(new ErrorResponse("The match was not found for this group."));
        }
        
        var vote = await groupDestinationVoteRepository.GetByMemberAndDestinationAsync(
            groupMember.Id, matchToRemove.DestinationId, cancellationToken);
        
        if (vote is null)
        {
            return NotFound(new ErrorResponse("The vote was not found."));
        }
        
        vote.SetApproved(false);
        
        groupMatchRepository.Remove(matchToRemove);
        groupDestinationVoteRepository.Update(vote);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return NoContent();
    }
}
