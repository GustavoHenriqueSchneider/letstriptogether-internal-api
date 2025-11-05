using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

public class GetGroupMemberAllDestinationVotesByIdQuery : IRequest<GetGroupMemberAllDestinationVotesByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
