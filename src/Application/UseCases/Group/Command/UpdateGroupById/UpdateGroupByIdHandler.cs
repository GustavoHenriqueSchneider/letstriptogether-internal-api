using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Common;
using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;

public class UpdateGroupByIdHandler(
    IGroupRepository groupRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository)
    : IRequestHandler<UpdateGroupByIdCommand>
{
    public async Task Handle(UpdateGroupByIdCommand request, CancellationToken cancellationToken)
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
            throw new BadRequestException("Only the group owner can update group data.");
        }

        group.Update(request.Name, request.TripExpectedDate);
        groupRepository.Update(group);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}
