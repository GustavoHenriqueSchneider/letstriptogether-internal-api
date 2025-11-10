using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.GroupMember.Command.RemoveGroupMemberById;

public class RemoveGroupMemberByIdHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<RemoveGroupMemberByIdCommand>
{
    public async Task Handle(RemoveGroupMemberByIdCommand request, CancellationToken cancellationToken)
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
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(userToRemove);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
