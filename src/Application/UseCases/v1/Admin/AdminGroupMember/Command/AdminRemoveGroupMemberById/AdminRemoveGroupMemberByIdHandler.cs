using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Common;
using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupPreferenceRepository groupPreferenceRepository,
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AdminRemoveGroupMemberByIdCommand>
{
    public async Task Handle(AdminRemoveGroupMemberByIdCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var userToRemove = group.Members.SingleOrDefault(m => m.Id == request.MemberId);
        if (userToRemove is null)
        {
            throw new NotFoundException("The user is not a member of this group.");
        }

        if (userToRemove.IsOwner)
        {
            throw new BadRequestException("It is not possible to remove the owner of group.");
        }

        group.RemoveMember(userToRemove);
        
        groupRepository.Update(group);
        groupMemberRepository.Remove(userToRemove);
        groupPreferenceRepository.Update(group.Preferences);
        
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
