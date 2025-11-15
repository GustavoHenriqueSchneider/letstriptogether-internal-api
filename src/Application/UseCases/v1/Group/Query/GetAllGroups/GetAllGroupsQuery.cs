using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.Group.Query.GetAllGroups;

public class GetAllGroupsQuery : IRequest<GetAllGroupsResponse>
{
    [JsonIgnore] public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
