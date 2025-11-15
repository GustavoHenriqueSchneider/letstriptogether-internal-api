using Application.Common.Exceptions;
using Domain.Aggregates.GroupAggregate;
using Domain.Aggregates.UserAggregate;
using MediatR;

namespace Application.UseCases.v1.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdHandler(
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetGroupMatchByIdQuery, GetGroupMatchByIdResponse>
{
    public async Task<GetGroupMatchByIdResponse> Handle(GetGroupMatchByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            throw new NotFoundException("User not found.");
        }

        var group = await groupRepository.GetGroupWithMatchesAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(request.GroupId, currentUserId, cancellationToken);

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
