using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdHandler : IRequestHandler<GetGroupMemberByIdQuery, GetGroupMemberByIdResponse>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupMemberByIdHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetGroupMemberByIdResponse> Handle(GetGroupMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await _userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var isGroupMember = user.GroupMemberships.Any(m => m.GroupId == request.GroupId);

        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

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
