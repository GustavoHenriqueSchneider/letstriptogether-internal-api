using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests;
using WebApi.DTOs.Responses;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers;

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
    [HttpDelete]
    public async Task<IActionResult> RemoveUserFromGroup(
        [FromRoute] Guid groupId, 
        [FromBody] RemoveUserFromGroupRequest request)
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

        var userToRemove = group.Members.SingleOrDefault(m => m.UserId == request.UserId);
        if (userToRemove is null)
        {
            return NotFound(new ErrorResponse("The user is not a member of this group."));
        }

        groupMemberRepository.Remove(userToRemove);
        await unitOfWork.SaveAsync();

        return NoContent();
    }
}