using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

public class GetGroupMemberAllDestinationVotesByIdHandler(
    IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetGroupMemberAllDestinationVotesByIdQuery, GetGroupMemberAllDestinationVotesByIdResponse>
{
    public async Task<GetGroupMemberAllDestinationVotesByIdResponse> Handle(GetGroupMemberAllDestinationVotesByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == request.GroupId);

        if (groupMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var group = await groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var (votes, hits) = await groupMemberDestinationVoteRepository.GetByMemberIdAsync(groupMember.Id,
            request.PageNumber, request.PageSize, cancellationToken);

        return new GetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = votes.Select(x => new GetGroupMemberAllDestinationVotesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
