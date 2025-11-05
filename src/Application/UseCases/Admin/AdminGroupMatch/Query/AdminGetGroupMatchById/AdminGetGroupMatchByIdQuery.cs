using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdQuery : IRequest<AdminGetGroupMatchByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
}
