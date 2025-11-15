using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupQuery : IRequest<GetNotVotedDestinationsByMemberOnGroupResponse>
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
