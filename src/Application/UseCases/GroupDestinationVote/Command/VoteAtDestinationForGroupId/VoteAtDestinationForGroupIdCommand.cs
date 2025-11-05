using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;

public record VoteAtDestinationForGroupIdCommand : IRequest<VoteAtDestinationForGroupIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationId { get; init; }
    public bool IsApproved { get; init; }
    public Guid UserId { get; init; }
}
