namespace WebApi.Models;

public class Group : TrackableEntity
{
    public string Name { get; set; } = null!;
    public DateTime TripExpectedDate { get; set; }
    // TODO: fazer listas readonly
    public List<GroupInvitation> Invitations { get; init; } = [];
    public List<GroupMatch> Matches { get; init; } = [];
    public List<GroupMember> Members { get; init; } = [];

    private Group() { }

    public Group(string name, DateTime tripExpectedDate)
    {
        Name = name;
        TripExpectedDate = tripExpectedDate;
    }

    public void Update(string name, DateTime tripExpectedDate)
    {
        Name = name;
        TripExpectedDate = tripExpectedDate;
        SetUpdateAt(DateTime.UtcNow);
    }
}
