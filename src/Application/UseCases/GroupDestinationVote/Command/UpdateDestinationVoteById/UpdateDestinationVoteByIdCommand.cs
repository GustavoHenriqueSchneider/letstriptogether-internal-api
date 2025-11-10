using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public record UpdateDestinationVoteByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
    public bool IsApproved { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
