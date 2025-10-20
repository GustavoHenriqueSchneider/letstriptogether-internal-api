namespace WebApi.Models;

public class GroupMember : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public bool IsOwner { get; init; }
    // TODO: fazer lista readonly
    public List<GroupMemberDestinationVote> Votes { get; init; } = [];

    private GroupMember() { }
}
