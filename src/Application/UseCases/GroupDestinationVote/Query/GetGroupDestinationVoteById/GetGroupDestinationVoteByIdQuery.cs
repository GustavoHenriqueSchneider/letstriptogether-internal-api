using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

public class GetGroupDestinationVoteByIdQuery : IRequest<GetGroupDestinationVoteByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid DestinationVoteId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
