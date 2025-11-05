using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdHandler : IRequestHandler<GetGroupMatchByIdQuery, GetGroupMatchByIdResponse>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GetGroupMatchByIdHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GetGroupMatchByIdResponse> Handle(GetGroupMatchByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var existsUser = await _userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            throw new NotFoundException("User not found.");
        }

        var group = await _groupRepository.GetGroupWithMatchesAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = await _groupRepository.IsGroupMemberByUserIdAsync(request.GroupId, currentUserId, cancellationToken);

        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var groupMatch = group.Matches.SingleOrDefault(x => x.Id == request.MatchId);

        if (groupMatch is null)
        {
            throw new NotFoundException("Group match not found.");
        }

        return new GetGroupMatchByIdResponse
        {
            DestinationId = groupMatch.DestinationId,
            CreatedAt = groupMatch.CreatedAt,
            UpdatedAt = groupMatch.UpdatedAt
        };
    }
}
