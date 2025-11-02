using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Admin.GroupMatch;
using WebApi.DTOs.Responses.GroupMatch;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/matches")]
public class GroupMatchController(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroupMatchesById([FromRoute] Guid groupId,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId);

        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var (groupMatches, hits) =
            await groupMatchRepository.GetByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new GetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits > 0 ? hits - 1 : hits
        });
    }

    [HttpGet("{matchId:guid}")]
    public async Task<IActionResult> GetGroupMatchById([FromRoute] Guid groupId,
        [FromRoute] Guid matchId)
    {
        var currentUserId = currentUser.GetId();
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId);

        if (!existsUser)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMatchesAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId);

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
        [FromRoute] Guid matchId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMatchesAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isMember = await groupRepository.IsGroupMemberByUserIdAsync(groupId, currentUserId);

        if (!isMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var matchToRemove = group.Matches.SingleOrDefault(m => m.Id == matchId);

        if (matchToRemove is null)
        {
            return NotFound(new ErrorResponse("The match was not found for this group."));
        }

        groupMatchRepository.Remove(matchToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}
