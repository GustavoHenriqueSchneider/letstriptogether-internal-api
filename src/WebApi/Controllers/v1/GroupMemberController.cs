using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/members")]
public class GroupMemberController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOtherGroupMembersById([FromRoute] Guid groupId, 
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var isGroupMember = user.GroupMemberships.Any(m => m.GroupId == groupId);

        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var groupExists = await groupRepository.ExistsByIdAsync(groupId);

        if (!groupExists)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var (groupMembers, hits) = 
            await groupMemberRepository.GetAllByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetOtherGroupMembersByIdResponse
        {
            Data = groupMembers
                .Where(x => x.UserId != currentUserId)
                .Select(x => new GetOtherGroupMembersByIdResponseData
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt
                }),
            Hits = hits > 0 ? hits - 1 : hits
        });
    }

    [HttpGet("{memberId:guid}")]
    public async Task<IActionResult> GetGroupMemberById([FromRoute] Guid groupId, 
        [FromRoute] Guid memberId)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId);

        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var isGroupMember = user.GroupMemberships.Any(m => m.GroupId == groupId);

        if (!isGroupMember)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == memberId);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new GetGroupMemberByIdResponse
        {
            Name = groupMember.User.Name,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid memberId)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(groupId);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            return BadRequest(new ErrorResponse("You are not a member of this group."));
        }

        if (!currentUserMember.IsOwner)
        {
            return BadRequest(new ErrorResponse("Only the group owner can remove members."));
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == memberId);
        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        if (currentUser.GetId() == userToRemove.UserId)
        {
            return BadRequest(new ErrorResponse("User can not remove itself."));
        }

        group.RemoveMember(userToRemove);
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(userToRemove);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync();
        return NoContent();
    }
}