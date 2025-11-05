using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

public class CancelActiveGroupInvitationHandler : IRequestHandler<CancelActiveGroupInvitationCommand>
{
    private readonly IGroupInvitationRepository _groupInvitationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CancelActiveGroupInvitationHandler(
        IGroupInvitationRepository groupInvitationRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupInvitationRepository = groupInvitationRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(CancelActiveGroupInvitationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);
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
            await _groupInvitationRepository.GetByGroupAndStatusAsync(request.GroupId, GroupInvitationStatus.Active, cancellationToken);
        
        if (activeInvitation is null)
        {
            throw new NotFoundException("Active invitation not found.");
        }

        activeInvitation.Cancel();
        _groupInvitationRepository.Update(activeInvitation);
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
