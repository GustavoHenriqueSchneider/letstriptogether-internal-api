using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;

public class RemoveGroupMemberByIdHandler : IRequestHandler<RemoveGroupMemberByIdCommand>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public RemoveGroupMemberByIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(RemoveGroupMemberByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await _groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
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
            throw new BadRequestException("Only the group owner can remove members.");
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == request.MemberId);
        if (userToRemove is null)
        {
            throw new NotFoundException("The user is not a member of this group.");
        }

        if (currentUserId == userToRemove.UserId)
        {
            throw new BadRequestException("User can not remove itself.");
        }

        group.RemoveMember(userToRemove);
        
        _groupRepository.Update(group);
        _groupMemberRepository.Remove(userToRemove);
        _groupPreferenceRepository.Update(group.Preferences);
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
