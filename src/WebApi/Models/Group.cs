namespace WebApi.Models;

public class Group : TrackableEntity
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
    public List<GroupInvitation> Invitations { get; init; } = [];
    public List<GroupMatch> Matches { get; init; } = [];
    public List<GroupMember> Members { get; init; } = [];
}
