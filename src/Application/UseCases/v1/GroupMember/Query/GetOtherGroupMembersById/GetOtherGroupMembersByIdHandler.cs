using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;

public class GetOtherGroupMembersByIdHandler(
    IGroupMemberRepository groupMemberRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetOtherGroupMembersByIdQuery, GetOtherGroupMembersByIdResponse>
{
    public async Task<GetOtherGroupMembersByIdResponse> Handle(GetOtherGroupMembersByIdQuery request, CancellationToken cancellationToken)
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

        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var (groupMembers, hits) = 
            await groupMemberRepository.GetAllByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

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
