using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;

public class RemoveGroupMatchByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
    public Guid UserId { get; init; }
}
