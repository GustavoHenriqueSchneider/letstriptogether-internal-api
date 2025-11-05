using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.DeleteGroupById;

public class DeleteGroupByIdHandler : IRequestHandler<DeleteGroupByIdCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public DeleteGroupByIdHandler(
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteGroupByIdCommand request, CancellationToken cancellationToken)
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
            throw new BadRequestException("Only the group owner can delete the group.");
        }

        _groupRepository.Remove(group);
        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
