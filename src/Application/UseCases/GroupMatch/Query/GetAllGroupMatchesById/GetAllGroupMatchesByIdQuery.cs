using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

public class GetAllGroupMatchesByIdQuery : IRequest<GetAllGroupMatchesByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
