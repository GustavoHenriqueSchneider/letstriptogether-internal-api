using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupInvitation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/groups/{groupId:guid}/invitations")]
public class GroupInvitationController(
    IGroupRepository groupRepository,
    IGroupInvitationRepository groupInvitationRepository,
    IApplicationUserContextExtensions currentUser,
    IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);
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
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (existingInvitation is not null)
        {
            groupInvitation = existingInvitation.Copy();
            groupInvitationRepository.Update(existingInvitation);
        }
        else
        {
            groupInvitation = new GroupInvitation(groupId);
        }

        await groupInvitationRepository.AddAsync(groupInvitation, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        var invitationToken = tokenService.GenerateInvitationToken(groupInvitation.Id);

        return CreatedAtAction(nameof(CreateGroupInvitation), 
            new CreateGroupInvitationResponse { Token = invitationToken });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetActiveGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);
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
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (activeInvitation is null)
        {
            return NotFound(new ErrorResponse("Active invitation not found."));
        }
        
        var invitationToken = tokenService.GenerateInvitationToken(activeInvitation.Id);
        return Ok(new GetActiveGroupInvitationResponse { Token =  invitationToken });
    }

    [HttpPatch("cancel")]
    public async Task<IActionResult> CancelActiveGroupInvitation([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            return NotFound(new ErrorResponse("User not found."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupId, cancellationToken);
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
            await groupInvitationRepository.GetByGroupAndStatusAsync(groupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (activeInvitation is null)
        {
            return NotFound(new ErrorResponse("Active invitation not found."));
        }

        activeInvitation.Cancel();
        groupInvitationRepository.Update(activeInvitation);
        
        await unitOfWork.SaveAsync(cancellationToken);
        return NoContent();
    }
}
