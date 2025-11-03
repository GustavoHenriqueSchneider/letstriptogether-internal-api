using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Context.Interfaces;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.GroupInvitation;
using WebApi.Models.Aggregates;
using WebApi.Models.Enums;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Services.Interfaces;

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
    ITokenService tokenService,
    IRedisService redisService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] Guid groupId)
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

        GroupInvitation groupInvitation;
        
        var existingInvitation = 
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active);
        
        if (existingInvitation is not null)
        {
            groupInvitation = existingInvitation.Copy();
            groupInvitationRepository.Update(existingInvitation);
        }
        else
        {
            groupInvitation = new GroupInvitation(groupId);
        }

        await groupInvitationRepository.AddAsync(groupInvitation);
        await unitOfWork.SaveAsync();
        
        var invitationToken = tokenService.GenerateInvitationToken(groupInvitation.Id);

        return CreatedAtAction(nameof(CreateGroupInvitation), 
            new CreateGroupInvitationResponse { Token = invitationToken });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetActiveGroupInvitation([FromRoute] Guid groupId)
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
            return BadRequest(new ErrorResponse("Only the group owner can get group invitation."));
        }

        var activeInvitation = 
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active);
        
        if (activeInvitation is null)
        {
            return NotFound(new ErrorResponse("Active invitation not found."));
        }
        
        var invitationToken = tokenService.GenerateInvitationToken(activeInvitation.Id);
        return Ok(new GetActiveGroupInvitationResponse { Token =  invitationToken });
    }

    [HttpPatch("cancel")]
    public async Task<IActionResult> CancelActiveGroupInvitation([FromRoute] Guid groupId)
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
            return BadRequest(new ErrorResponse("Only the group owner can get group invitation."));
        }

        var activeInvitation = 
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active);
        
        if (activeInvitation is null)
        {
            return NotFound(new ErrorResponse("Active invitation not found."));
        }

        activeInvitation.Cancel();
        groupInvitationRepository.Update(activeInvitation);
        
        await unitOfWork.SaveAsync();
        return NoContent();
    }
}
