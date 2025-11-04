using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Invitation;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/invitations")]
public class InvitationController(
    IUnitOfWork unitOfWork,
    IApplicationUserContext currentUser,
    IUserRepository userRepository,
    IGroupInvitationRepository groupInvitationRepository,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IGroupRepository groupRepository,
    IGroupMemberRepository groupMemberRepository,
    ITokenService tokenService,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupMatchRepository groupMatchRepository): ControllerBase
{
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);
        
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }
        
        if (user.Preferences is null)
        {
            return BadRequest(new ErrorResponse("User has not filled any preferences yet."));
        }
        
        _ = new UserPreference(user.Preferences);

        var (isValid, id) = tokenService.ValidateInvitationToken(request.Token);

        if (!isValid)
        {
            return Unauthorized(new ErrorResponse("Invalid invitation token."));
        }

        var (isExpired, _) = tokenService.IsTokenExpired(request.Token);

        if (isExpired)
        {
            return Unauthorized(new ErrorResponse("Invitation token has expired."));
        }

        if (!Guid.TryParse(id, out var invitationId) || invitationId == Guid.Empty)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }
        
        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId, cancellationToken);
        if (groupInvitation is null)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }
        
        if (groupInvitation.Status != GroupInvitationStatus.Active)
        {
            return BadRequest(new ErrorResponse("Invitation is not active."));
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            groupInvitation.Expire();
            
            groupInvitationRepository.Update(groupInvitation);
            await unitOfWork.SaveAsync(cancellationToken);
            
            return BadRequest(new ErrorResponse("Invitation is not active."));
        }

        var existingAnswer = groupInvitation.AnsweredBy.Any(x => x.UserId == currentUserId);
        if (existingAnswer)
        {
            return Conflict(new ErrorResponse("You have already answered this invitation."));
        }

        var group = await groupRepository.GetGroupWithMembersAndMatchesAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            return BadRequest(new ErrorResponse("You are already a member of this group."));
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: true);
        var groupMember = group.AddMember(user, false);
        
        await userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await groupMemberRepository.AddAsync(groupMember, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        var groupToUpdate = await groupRepository.GetGroupWithMembersPreferencesAsync(group.Id, cancellationToken);
        if (groupToUpdate is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }
        
        groupToUpdate.UpdatePreferences();
        
        groupRepository.Update(groupToUpdate);
        groupPreferenceRepository.Update(groupToUpdate.Preferences);

        await unitOfWork.SaveAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("refuse")]
    public async Task<IActionResult> RefuseInvitation([FromBody] RefuseInvitationRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetId();
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);
        
        if (user is null)
        {
            return NotFound(new ErrorResponse("User not found."));
        }
        
        if (user.Preferences is null)
        {
            return BadRequest(new ErrorResponse("User has not filled any preferences yet."));
        }

        var (isValid, id) = tokenService.ValidateInvitationToken(request.Token);

        if (!isValid)
        {
            return Unauthorized(new ErrorResponse("Invalid invitation token."));
        }

        var (isExpired, _) = tokenService.IsTokenExpired(request.Token);

        if (isExpired)
        {
            return Unauthorized(new ErrorResponse("Invitation token has expired."));
        }

        if (!Guid.TryParse(id, out var invitationId) || invitationId == Guid.Empty)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }
        
        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId, cancellationToken);
        if (groupInvitation is null)
        {
            return NotFound(new ErrorResponse("Invitation not found."));
        }
        
        if (groupInvitation.Status != GroupInvitationStatus.Active)
        {
            return BadRequest(new ErrorResponse("Invitation is not active."));
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            groupInvitation.Expire();
            
            groupInvitationRepository.Update(groupInvitation);
            await unitOfWork.SaveAsync(cancellationToken);
            
            return BadRequest(new ErrorResponse("Invitation is not active."));
        }

        var existingAnswer = groupInvitation.AnsweredBy.Any(x => x.UserId == currentUserId);
        if (existingAnswer)
        {
            return Conflict(new ErrorResponse("You have already answered this invitation."));
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            return NotFound(new ErrorResponse("Group not found."));
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            return BadRequest(new ErrorResponse("You are already a member of this group."));
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: false);
        
        await userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        return Ok();
    }
}
