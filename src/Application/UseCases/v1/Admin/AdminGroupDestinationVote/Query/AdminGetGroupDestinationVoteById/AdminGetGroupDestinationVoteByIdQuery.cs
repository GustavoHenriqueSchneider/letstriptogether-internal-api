using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

public class AdminGetGroupDestinationVoteByIdQuery : IRequest<AdminGetGroupDestinationVoteByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
}
