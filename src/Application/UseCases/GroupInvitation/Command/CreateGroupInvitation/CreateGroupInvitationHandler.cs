using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.GroupAggregate.Enums;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;
using GroupInvitationModel = Domain.Aggregates.GroupAggregate.Entities.GroupInvitation;

namespace Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationHandler(
    IGroupInvitationRepository groupInvitationRepository,
    IGroupRepository groupRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<CreateGroupInvitationCommand, CreateGroupInvitationResponse>
{
    public async Task<CreateGroupInvitationResponse> Handle(CreateGroupInvitationCommand request, CancellationToken cancellationToken)
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
            throw new BadRequestException("Only the group owner can create invitations.");
        }

        GroupInvitationModel groupInvitation;
        
        var existingInvitation = 
            await groupInvitationRepository.GetByGroupAndStatusAsync(request.GroupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (existingInvitation is not null)
        {
            groupInvitation = existingInvitation.Copy();
            groupInvitationRepository.Update(existingInvitation);
        }
        else
        {
            groupInvitation = new GroupInvitationModel(request.GroupId);
        }

        await groupInvitationRepository.AddAsync(groupInvitation, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        
        var invitationToken = tokenService.GenerateInvitationToken(groupInvitation.Id);

        return new CreateGroupInvitationResponse { Token = invitationToken };
    }
}
