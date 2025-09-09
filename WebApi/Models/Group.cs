namespace WebApi.Models;

public class Group : TrackableEntity
{
    public string Name { get; set; } = null!;
    public DateTime ExpectedDate { get; set; }
    public List<GroupMember> Members { get; set; } = new();
    public List<GroupMatch> Matches { get; set; } = new();
    public List<GroupInvitation> Invitations { get; set; } = new();
}
