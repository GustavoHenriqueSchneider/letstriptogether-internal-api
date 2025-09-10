namespace WebApi.Models;

public class GroupMember : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public List<GroupMemberDestinationVote> Votes { get; init; } = [];
}
