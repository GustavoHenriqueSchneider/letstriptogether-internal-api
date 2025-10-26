namespace WebApi.DTOs.Responses.Admin.GroupMemberDestinationVote;

public class AdminGetGroupDestinationVoteByIdResponse
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public Guid DestinationId { get; init; }
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
