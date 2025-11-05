using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsQuery : IRequest<AdminGetAllGroupsResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
