using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

public class AdminGetAllGroupDestinationVotesByIdQuery : IRequest<AdminGetAllGroupDestinationVotesByIdResponse>
{
    public Guid GroupId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
