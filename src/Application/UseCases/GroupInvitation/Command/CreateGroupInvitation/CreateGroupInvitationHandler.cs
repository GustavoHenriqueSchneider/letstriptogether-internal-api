using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using GroupInvitationModel = LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupInvitation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

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
