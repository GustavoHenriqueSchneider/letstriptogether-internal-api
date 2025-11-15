using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdQuery : IRequest<AdminGetGroupMatchByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MatchId { get; init; }
}
