using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdQuery : IRequest<GetGroupMatchByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
    public Guid UserId { get; init; }
}
