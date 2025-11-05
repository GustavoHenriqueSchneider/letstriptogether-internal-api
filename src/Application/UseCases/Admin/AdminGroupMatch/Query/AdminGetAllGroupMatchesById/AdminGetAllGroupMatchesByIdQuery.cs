using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdQuery : IRequest<AdminGetAllGroupMatchesByIdResponse>
{
    public Guid GroupId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
