using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Requests.Group;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Group;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/invitations")]
public class GroupInvitationController(
    IGroupRepository groupRepository,
    IGroupInvitationRepository groupInvitationRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupMemberRepository groupMemberRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] Guid groupId, 
        [FromBody] CreateGroupInvitationRequest request)
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
            return BadRequest(new ErrorResponse("Only the group owner can create invitations."));
        }

        var existingInvitation = await groupInvitationRepository.GetByGroupIdAsync(groupId);

        if (existingInvitation is not null)
        {
            return Conflict(new ErrorResponse("An invitation already exists for this group."));
        }

        var groupInvitation = (GroupInvitation)Activator.CreateInstance(
            typeof(GroupInvitation),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [],
            null)!;
        
        typeof(GroupInvitation).GetProperty(nameof(GroupInvitation.GroupId))!
            .SetValue(groupInvitation, groupId);
        typeof(GroupInvitation).GetProperty(nameof(GroupInvitation.ExpirationDate))!
            .SetValue(groupInvitation, request.ExpirationDate.ToUniversalTime());

        await groupInvitationRepository.AddAsync(groupInvitation);
        await unitOfWork.SaveAsync();

        return CreatedAtAction(nameof(CreateGroupInvitation), 
            new CreateGroupInvitationResponse { Id = groupInvitation.Id });
    }

    [HttpDelete("{invitationId:guid}")]
    public async Task<IActionResult> RemoveGroupInvitation([FromRoute] Guid groupId, 
        [FromRoute] Guid invitationId)
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
            return BadRequest(new ErrorResponse("Only the group owner can remove invitations."));
        }

        var groupInvitation = await groupInvitationRepository.GetByIdAsync(invitationId);

        if (groupInvitation is null)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }

        if (groupInvitation.GroupId != groupId)
        {
            return BadRequest(new ErrorResponse("Invitation does not belong to this group."));
        }

        groupInvitationRepository.Remove(groupInvitation);
        await unitOfWork.SaveAsync();

        return NoContent();
    }

    [HttpPost("{invitationId:guid}/accept")]
    public async Task<IActionResult> AcceptGroupInvitation([FromRoute] Guid groupId, 
        [FromRoute] Guid invitationId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId);

        if (groupInvitation is null)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }

        if (groupInvitation.GroupId != groupId)
        {
            return BadRequest(new ErrorResponse("Invitation does not belong to this group."));
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            return BadRequest(new ErrorResponse("Invitation has expired."));
        }

        var existingAnswer = await userGroupInvitationRepository.GetByUserIdAndGroupInvitationIdAsync(
            currentUserId, invitationId);

        if (existingAnswer is not null)
        {
            return Conflict(new ErrorResponse("You have already answered this invitation."));
        }

        var groupWithMembers = await groupRepository.GetGroupWithMembersAsync(groupId);

        if (groupWithMembers is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isAlreadyMember = groupWithMembers.Members.Any(m => m.UserId == currentUserId);

        if (isAlreadyMember)
        {
            return BadRequest(new ErrorResponse("You are already a member of this group."));
        }

        var userGroupInvitation = (UserGroupInvitation)Activator.CreateInstance(
            typeof(UserGroupInvitation),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [],
            null)!;
        
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.GroupInvitationId))!
            .SetValue(userGroupInvitation, invitationId);
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.UserId))!
            .SetValue(userGroupInvitation, currentUserId);
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.IsAccepted))!
            .SetValue(userGroupInvitation, true);

        await userGroupInvitationRepository.AddAsync(userGroupInvitation);

        var groupMember = new GroupMember
        {
            GroupId = groupId,
            UserId = currentUserId,
            IsOwner = false
        };

        await groupMemberRepository.AddAsync(groupMember);

        await unitOfWork.SaveAsync();

        return Ok();
    }

    [HttpPost("{invitationId:guid}/refuse")]
    public async Task<IActionResult> RefuseGroupInvitation([FromRoute] Guid groupId, 
        [FromRoute] Guid invitationId)
    {
        var currentUserId = currentUser.GetId();

        if (!await userRepository.ExistsByIdAsync(currentUserId))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId);

        if (groupInvitation is null)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }

        if (groupInvitation.GroupId != groupId)
        {
            return BadRequest(new ErrorResponse("Invitation does not belong to this group."));
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            return BadRequest(new ErrorResponse("Invitation has expired."));
        }

        var existingAnswer = await userGroupInvitationRepository.GetByUserIdAndGroupInvitationIdAsync(
            currentUserId, invitationId);

        if (existingAnswer is not null)
        {
            return Conflict(new ErrorResponse("You have already answered this invitation."));
        }

        var userGroupInvitation = (UserGroupInvitation)Activator.CreateInstance(
            typeof(UserGroupInvitation),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [],
            null)!;
        
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.GroupInvitationId))!
            .SetValue(userGroupInvitation, invitationId);
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.UserId))!
            .SetValue(userGroupInvitation, currentUserId);
        typeof(UserGroupInvitation).GetProperty(nameof(UserGroupInvitation.IsAccepted))!
            .SetValue(userGroupInvitation, false);

        await userGroupInvitationRepository.AddAsync(userGroupInvitation);
        await unitOfWork.SaveAsync();

        return Ok();
    }
}

