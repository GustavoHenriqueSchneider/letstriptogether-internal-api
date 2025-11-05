using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

public class GetGroupDestinationVoteByIdQuery : IRequest<GetGroupDestinationVoteByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
    public Guid UserId { get; init; }
}
