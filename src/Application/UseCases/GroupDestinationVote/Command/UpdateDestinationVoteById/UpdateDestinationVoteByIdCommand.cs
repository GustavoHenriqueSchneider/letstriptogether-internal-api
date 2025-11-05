using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

public record UpdateDestinationVoteByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
    public bool IsApproved { get; init; }
    public Guid UserId { get; init; }
}
