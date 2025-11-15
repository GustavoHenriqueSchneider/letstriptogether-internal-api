using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsQuery : IRequest<AdminGetAllGroupsResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
