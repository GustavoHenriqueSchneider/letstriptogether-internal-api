using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
