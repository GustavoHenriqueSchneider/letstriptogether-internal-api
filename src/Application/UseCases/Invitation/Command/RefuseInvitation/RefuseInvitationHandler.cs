using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Enums;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Invitation.Command.RefuseInvitation;

public class RefuseInvitationHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IUserGroupInvitationRepository userGroupInvitationRepository,
    IUserRepository userRepository)
    : IRequestHandler<RefuseInvitationCommand>
{
    public async Task Handle(RefuseInvitationCommand request, CancellationToken cancellationToken)
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

        var group = await groupRepository.GetGroupWithMembersAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isAlreadyMember = group.Members.Any(x => x.UserId == currentUserId);
        if (isAlreadyMember)
        {
            throw new BadRequestException("You are already a member of this group.");
        }
        
        var invitationAnswer = groupInvitation.AddAnswer(currentUserId, isAccepted: false);
        
        await userGroupInvitationRepository.AddAsync(invitationAnswer, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
