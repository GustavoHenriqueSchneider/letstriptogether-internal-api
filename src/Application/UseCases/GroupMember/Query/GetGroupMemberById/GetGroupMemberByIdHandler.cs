using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdHandler(
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetGroupMemberByIdQuery, GetGroupMemberByIdResponse>
{
    public async Task<GetGroupMemberByIdResponse> Handle(GetGroupMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var isGroupMember = user.GroupMemberships.Any(m => m.GroupId == request.GroupId);

        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var groupMember = group.Members.SingleOrDefault(x => x.Id == request.MemberId);

        if (groupMember is null)
        {
            throw new NotFoundException("Group member not found.");
        }

        return new GetGroupMemberByIdResponse
        {
            Name = groupMember.User.Name,
            IsOwner = groupMember.IsOwner,
            CreatedAt = groupMember.CreatedAt,
            UpdatedAt = groupMember.UpdatedAt
        };
    }
}
