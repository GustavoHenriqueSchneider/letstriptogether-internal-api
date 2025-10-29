namespace WebApi.DTOs.Responses.GroupMemberDestinationVote;

public class GetGroupDestinationVoteByIdResponse
{
    public Guid DestinationId { get; init; }
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
