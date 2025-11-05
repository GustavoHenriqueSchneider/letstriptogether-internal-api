using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

public class GetGroupMemberAllDestinationVotesByIdHandler : IRequestHandler<GetGroupMemberAllDestinationVotesByIdQuery, GetGroupMemberAllDestinationVotesByIdResponse>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupMemberAllDestinationVotesByIdHandler(
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetGroupMemberAllDestinationVotesByIdResponse> Handle(GetGroupMemberAllDestinationVotesByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var user = await _userRepository.GetUserWithGroupMembershipsAsync(currentUserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var groupMember = user.GroupMemberships.SingleOrDefault(m => m.GroupId == request.GroupId);

        if (groupMember is null)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var group = await _groupRepository.GetGroupWithMembersAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var (votes, hits) = await _groupMemberDestinationVoteRepository.GetByMemberIdAsync(groupMember.Id,
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
