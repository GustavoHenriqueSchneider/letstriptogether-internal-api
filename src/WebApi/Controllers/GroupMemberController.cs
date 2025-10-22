using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.GroupMember;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupMember;
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
    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetAllGroupMembers([FromRoute] Guid groupId, 
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (groupMembers, hits) = 
            await groupMemberRepository.GetAllByGroupIdAsync(groupId, pageNumber, pageSize);

        return Ok(new GetAllGroupMembersResponse
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
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> GetGroupMemberById([FromRoute] Guid groupId, [FromRoute] Guid id)
    {
        var group = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var groupMember = group.Members.SingleOrDefault(x => x.UserId == id);

        if (groupMember is null)
        {
            return NotFound(new ErrorResponse("Group member not found."));
        }

        return Ok(new GetGroupMemberByIdResponse
        {
            UserId = groupMember.UserId,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        });
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveGroupMemberById([FromRoute] Guid groupId,
        [FromBody] RemoveGroupMemberByIdRequest request)
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