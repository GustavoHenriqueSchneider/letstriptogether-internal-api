using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;

public class GetAllGroupsQuery : IRequest<GetAllGroupsResponse>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
