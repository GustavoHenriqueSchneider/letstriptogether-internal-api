namespace WebApi.Models;

public class GroupMemberDestinationVote : TrackableEntity
{
    public Guid GroupMemberId { get; init; }
    public GroupMember GroupMember { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public Destination Destination { get; init; } = null!;
    public bool IsApproved { get; init; }
}
