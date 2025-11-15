using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Enums;
using Domain.Common;
using MediatR;

namespace Application.UseCases.Invitation.Query.GetInvitationDetails;

public class GetInvitationDetailsHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GetInvitationDetailsQuery, GetInvitationDetailsResponse>
{
    public async Task<GetInvitationDetailsResponse> Handle(GetInvitationDetailsQuery request, CancellationToken cancellationToken)
    {
        var (isValid, invitationIdValue) = tokenService.ValidateInvitationToken(request.Token);
        if (!isValid)
        {
            throw new UnauthorizedException("Invalid invitation token.");
        }

        var (isExpired, _) = tokenService.IsTokenExpired(request.Token);
        if (isExpired)
        {
            throw new UnauthorizedException("Invitation token has expired.");
        }

        if (!Guid.TryParse(invitationIdValue, out var invitationId) || invitationId == Guid.Empty)
        {
            throw new NotFoundException("Invitation not found.");
        }

        var groupInvitation = await groupInvitationRepository.GetByIdWithAnsweredByAsync(invitationId, cancellationToken);
        if (groupInvitation is null)
        {
            throw new NotFoundException("Invitation not found.");
        }

        var group = await groupRepository.GetGroupWithMembersAsync(groupInvitation.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var owner = group.Members.SingleOrDefault(m => m.IsOwner);
        if (owner?.User is null)
        {
            throw new NotFoundException("Group owner not found.");
        }

        var isActive = groupInvitation.Status == GroupInvitationStatus.Active &&
                       groupInvitation.ExpirationDate >= DateTime.UtcNow;

        if (groupInvitation.ExpirationDate < DateTime.UtcNow)
        {
            groupInvitation.Expire();
            
            groupInvitationRepository.Update(groupInvitation);
            await unitOfWork.SaveAsync(cancellationToken);
        }

        return new GetInvitationDetailsResponse
        {
            CreatedBy = owner.User.Name,
            GroupName = group.Name,
            IsActive = isActive
        };
    }
}
