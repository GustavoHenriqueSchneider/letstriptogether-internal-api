using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;

public class GetOtherGroupMembersByIdHandler : IRequestHandler<GetOtherGroupMembersByIdQuery, GetOtherGroupMembersByIdResponse>
{
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetOtherGroupMembersByIdHandler(
        IGroupMemberRepository groupMemberRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupMemberRepository = groupMemberRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetOtherGroupMembersByIdResponse> Handle(GetOtherGroupMembersByIdQuery request, CancellationToken cancellationToken)
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

        var groupExists = await _groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (groupMembers, hits) = 
            await _groupMemberRepository.GetAllByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new GetOtherGroupMembersByIdResponse
        {
            Data = groupMembers
                .Where(x => x.UserId != currentUserId)
                .Select(x => new GetOtherGroupMembersByIdResponseData
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt
                }),
            Hits = hits > 0 ? hits - 1 : hits
        };
    }
}
