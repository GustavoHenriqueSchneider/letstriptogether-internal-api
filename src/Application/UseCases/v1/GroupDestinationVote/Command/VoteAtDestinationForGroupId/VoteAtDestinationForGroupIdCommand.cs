using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public record VoteAtDestinationForGroupIdCommand : IRequest<VoteAtDestinationForGroupIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationId { get; init; }
    public bool IsApproved { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
