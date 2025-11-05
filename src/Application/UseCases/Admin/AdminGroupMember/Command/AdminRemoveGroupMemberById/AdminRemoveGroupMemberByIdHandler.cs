using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdHandler : IRequestHandler<AdminRemoveGroupMemberByIdCommand>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupPreferenceRepository _groupPreferenceRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminRemoveGroupMemberByIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupPreferenceRepository groupPreferenceRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupPreferenceRepository = groupPreferenceRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AdminRemoveGroupMemberByIdCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetGroupWithMembersPreferencesAsync(request.GroupId, cancellationToken);
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
        
        _groupRepository.Update(group);
        _groupMemberRepository.Remove(userToRemove);
        _groupPreferenceRepository.Update(group.Preferences);
        
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
