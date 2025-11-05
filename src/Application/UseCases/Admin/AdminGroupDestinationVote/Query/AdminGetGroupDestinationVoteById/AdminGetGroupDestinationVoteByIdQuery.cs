using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdQuery : IRequest<AdminGetGroupDestinationVoteByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
}
