using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsHandler : IRequestHandler<AdminGetAllGroupsQuery, AdminGetAllGroupsResponse>
{
    private readonly IGroupRepository _groupRepository;

    public AdminGetAllGroupsHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<AdminGetAllGroupsResponse> Handle(AdminGetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var (groups, hits) = await _groupRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new AdminGetAllGroupsResponse
        {
            Data = groups.Select(x => new AdminGetAllGroupsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        };
    }
}
