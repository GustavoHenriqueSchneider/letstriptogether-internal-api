using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.Group.Command.LeaveGroupById;

public class LeaveGroupByIdHandler(
    IGroupMatchRepository groupMatchRepository,
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<LeaveGroupByIdCommand>
{
    public async Task Handle(LeaveGroupByIdCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        if (!await userRepository.ExistsByIdAsync(currentUserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
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
            var matches = await groupMatchRepository.GetAllMatchesByGroupAsync(request.GroupId, cancellationToken);
            groupMatchRepository.RemoveRange(matches);
        }
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(currentUserMember);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
