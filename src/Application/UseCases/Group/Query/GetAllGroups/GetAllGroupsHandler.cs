using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;

public class GetAllGroupsHandler(IGroupRepository groupRepository)
    : IRequestHandler<GetAllGroupsQuery, GetAllGroupsResponse>
{
    public async Task<GetAllGroupsResponse> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var (groups, hits) = await groupRepository.GetAllGroupsByUserIdAsync(
            request.UserId, request.PageNumber, request.PageSize, cancellationToken);

        return new GetAllGroupsResponse
        {
            Data = groups.Select(x => new GetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
