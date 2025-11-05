using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

using MediatR;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

public class GetAllGroupMatchesByIdHandler(
    IGroupMatchRepository groupMatchRepository,
    IGroupRepository groupRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetAllGroupMatchesByIdQuery, GetAllGroupMatchesByIdResponse>
{
    public async Task<GetAllGroupMatchesByIdResponse> Handle(GetAllGroupMatchesByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = request.UserId;
        var existsUser = await userRepository.ExistsByIdAsync(currentUserId, cancellationToken);

        if (!existsUser)
        {
            throw new NotFoundException("User not found.");
        }

        var groupExists = await groupRepository.ExistsByIdAsync(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            throw new NotFoundException("Group not found.");
        }

        var isGroupMember = await groupRepository.IsGroupMemberByUserIdAsync(request.GroupId, currentUserId, cancellationToken);

        if (!isGroupMember)
        {
            throw new BadRequestException("You are not a member of this group.");
        }

        var (groupMatches, hits) =
            await groupMatchRepository.GetByGroupIdAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        return new GetAllGroupMatchesByIdResponse
        {
            Data = groupMatches.Select(x => new GetAllGroupMatchesByIdResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
