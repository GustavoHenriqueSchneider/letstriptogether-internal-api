namespace WebApi.DTOs.Responses.GroupMemberDestinationVote;

public class GetGroupMemberDestinationVoteByIdResponse
{
    public Guid Id { get; init; }
    public Guid GroupMemberId { get; init; }
    public string GroupMemberName { get; init; } = null!;
    public string GroupMemberEmail { get; init; } = null!;
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public string DestinationAddress { get; init; } = null!;
    public List<string> DestinationCategories { get; init; } = [];
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
