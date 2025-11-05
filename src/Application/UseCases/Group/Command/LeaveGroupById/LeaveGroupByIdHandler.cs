using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.LeaveGroupById;

public class LeaveGroupByIdHandler : IRequestHandler<LeaveGroupByIdCommand>
{
    private readonly IGroupMatchRepository _groupMatchRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public LeaveGroupByIdHandler(
        IGroupMatchRepository groupMatchRepository,
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupMatchRepository = groupMatchRepository;
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(LeaveGroupByIdCommand request, CancellationToken cancellationToken)
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

        if (currentUserMember.IsOwner)
        {
            throw new BadRequestException("The group owner can not leave the group, only delete it.");
        }

        group.RemoveMember(currentUserMember);
        
        if (group.Members.Count == 1)
        {
            var matches = await _groupMatchRepository.GetAllMatchesByGroupAsync(request.GroupId, cancellationToken);
            _groupMatchRepository.RemoveRange(matches);
        }
        
        _groupRepository.Update(group);
        _groupMemberRepository.Remove(currentUserMember);
        _groupPreferenceRepository.Update(group.Preferences);
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
