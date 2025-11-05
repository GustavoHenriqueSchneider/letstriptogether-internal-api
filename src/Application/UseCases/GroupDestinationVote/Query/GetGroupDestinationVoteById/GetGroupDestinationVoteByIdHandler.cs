using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

public class GetGroupDestinationVoteByIdHandler : IRequestHandler<GetGroupDestinationVoteByIdQuery, GetGroupDestinationVoteByIdResponse>
{
    private readonly IGroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupDestinationVoteByIdHandler(
        IGroupMemberDestinationVoteRepository groupMemberDestinationVoteRepository,
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupMemberDestinationVoteRepository = groupMemberDestinationVoteRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetGroupDestinationVoteByIdResponse> Handle(GetGroupDestinationVoteByIdQuery request, CancellationToken cancellationToken)
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

        var vote = await _groupMemberDestinationVoteRepository.GetByIdWithRelationsAsync(request.GroupId,
            request.DestinationVoteId, cancellationToken);

        if (vote is null)
        {
            throw new NotFoundException("Group member destination vote not found.");
        }

        if (vote.GroupMemberId != groupMember.Id)
        {
            throw new BadRequestException("You are not a owner of this vote.");
        }

        return new GetGroupDestinationVoteByIdResponse
        {
            DestinationId = vote.DestinationId,
            IsApproved = vote.IsApproved,
            CreatedAt = vote.CreatedAt,
            UpdatedAt = vote.UpdatedAt
        };
    }
}
