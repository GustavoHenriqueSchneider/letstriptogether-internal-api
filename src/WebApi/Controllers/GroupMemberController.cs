using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMember;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[ApiController]
[Route("api/v1/groups/{groupId:guid}/members")]
public class GroupMemberController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroupMembersById([FromRoute] Guid groupId, 
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

        return Ok(new AdminGetAllGroupMembersResponse
        {
            Data = groupMembers.Select(x => new GetAllGroupMembersResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGroupMemberById([FromRoute] Guid groupId, [FromRoute] Guid id)
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

        var groupMember = group.Members.SingleOrDefault(x => x.Id == id);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new AdminGetGroupMemberByIdResponse
        {
            UserId = groupMember.UserId,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveGroupMemberById([FromRoute] Guid groupId,
        [FromRoute] Guid id)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

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

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == id);

        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        if (currentUser.GetId() == userToRemove.UserId)
        {
            return BadRequest(new ErrorResponse("User can not remove itself."));
        }

        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}