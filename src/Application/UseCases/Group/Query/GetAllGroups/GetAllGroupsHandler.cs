using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;

public class GetAllGroupsHandler : IRequestHandler<GetAllGroupsQuery, GetAllGroupsResponse>
{
    private readonly IGroupRepository _groupRepository;

    public GetAllGroupsHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GetAllGroupsResponse> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var (groups, hits) = await _groupRepository.GetAllGroupsByUserIdAsync(
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
