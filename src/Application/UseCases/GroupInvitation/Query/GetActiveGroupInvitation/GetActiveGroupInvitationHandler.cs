using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Enums;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<GetActiveGroupInvitationQuery, GetActiveGroupInvitationResponse>
{
    public async Task<GetActiveGroupInvitationResponse> Handle(GetActiveGroupInvitationQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var currentUserMember = group.Members.SingleOrDefault(m => m.UserId == currentUserId);
        if (currentUserMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        if (!currentUserMember.IsOwner)
        {
            throw new BadRequestException("Only the group owner can get group invitation.");
        }

        var activeInvitation = 
            await groupInvitationRepository.GetByGroupAndStatusAsync(request.GroupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (activeInvitation is null)
        {
            throw new NotFoundException("Active invitation not found.");
        }
        
        var invitationToken = tokenService.GenerateInvitationToken(activeInvitation.Id);
        return new GetActiveGroupInvitationResponse { Token = invitationToken };
    }
}
