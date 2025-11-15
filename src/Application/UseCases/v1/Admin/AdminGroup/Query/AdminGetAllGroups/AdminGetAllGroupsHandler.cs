using Domain.Aggregates.GroupAggregate;
using MediatR;

namespace Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsHandler(IGroupRepository groupRepository)
    : IRequestHandler<AdminGetAllGroupsQuery, AdminGetAllGroupsResponse>
{
    public async Task<AdminGetAllGroupsResponse> Handle(AdminGetAllGroupsQuery request, CancellationToken cancellationToken)
    {
        var (groups, hits) = await groupRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

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
