using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;

public class AcceptInvitationHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IGroupInvitationRepository groupInvitationRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRepository userRepository)
    : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await userRepository.GetByIdWithPreferencesAsync(currentUserId, cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }
        
        if (user.Preferences is null)
        {
            throw new BadRequestException("User has not filled any preferences yet.");
        }
        
        _ = new UserPreference(user.Preferences);

        var (isValid, id) = tokenService.ValidateInvitationToken(request.Token);

        if (!isValid)
        {
            throw new UnauthorizedException("Invalid invitation token.");
        }

        var (isExpired, _) = tokenService.IsTokenExpired(request.Token);

        if (isExpired)
        {
            throw new UnauthorizedException("Invitation token has expired.");
        }

        if (!Guid.TryParse(id, out var invitationId) || invitationId == Guid.Empty)
        {
            throw new NotFoundException("Invitation not found.");
        }
        
        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId, cancellationToken);
        if (groupInvitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }
        
        if (groupInvitation.Status != GroupInvitationStatus.Active)
        {
            throw new BadRequestException("Invitation is not active.");
        }

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            groupInvitation.Expire();
            
            groupInvitationRepository.Update(groupInvitation);
            await unitOfWork.SaveAsync(cancellationToken);
            
            throw new BadRequestException("Invitation is not active.");
        }

        var existingAnswer = groupInvitation.AnsweredBy.Any(x => x.UserId == currentUserId);
        if (existingAnswer)
        {
            throw new ConflictException("You have already answered this invitation.");
        }

        var group = await groupRepository.GetGroupWithMembersAndMatchesAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            throw new BadRequestException("You are already a member of this group.");
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: true);
        var groupMember = group.AddMember(user, false);
        
        await userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await groupMemberRepository.AddAsync(groupMember, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        var groupToUpdate = await groupRepository.GetGroupWithMembersPreferencesAsync(group.Id, cancellationToken);
        if (groupToUpdate is null)
        {
            throw new NotFoundException("Group not found.");
        }
        
        groupToUpdate.UpdatePreferences();
        
        groupRepository.Update(groupToUpdate);
        groupPreferenceRepository.Update(groupToUpdate.Preferences);

        await unitOfWork.SaveAsync(cancellationToken);
    }
}
