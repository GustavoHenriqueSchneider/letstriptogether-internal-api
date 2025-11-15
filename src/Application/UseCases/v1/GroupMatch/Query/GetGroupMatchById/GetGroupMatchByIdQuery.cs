using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdQuery : IRequest<GetGroupMatchByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
