using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdQuery : IRequest<AdminGetAllGroupMatchesByIdResponse>
{
    public Guid GroupId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
