namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.GroupDestinationVote;

public class VoteAtDestinationForGroupIdRequest
{
    public Guid DestinationId { get; init; }
    public bool IsApproved { get; init; }
}