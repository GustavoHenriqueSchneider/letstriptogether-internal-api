namespace WebApi.Models;

public class Group : TrackableEntity
{
    public string Name { get; private set; } = null!;

    private DateTime _tripExpectedDate;
    public DateTime TripExpectedDate
    {
        get => _tripExpectedDate.ToUniversalTime();
        private set => _tripExpectedDate = value;
    }

    private readonly List<GroupInvitation> _invitations = [];
    public IReadOnlyCollection<GroupInvitation> Invitations => _invitations.AsReadOnly();

    private readonly List<GroupMatch> _matches = [];
    public IReadOnlyCollection<GroupMatch> Matches => _matches.AsReadOnly();

    private readonly List<GroupMember> _members = [];
    public IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();

    private Group() { }

    public Group(string name, DateTime tripExpectedDate)
    {
        Name = name;
        TripExpectedDate = tripExpectedDate;
    }

    public bool HasMember(GroupMember member)
    {
        return _members.Contains(member);
    }

    public void AddMember(GroupMember member)
    {
        if (HasMember(member))
        {
            throw new Exception("This member is already included on the group.");
        }

        _members.Add(member);
    }

    public void Update(string? name, DateTime? tripExpectedDate)
    {
        Name = name ?? Name;
        TripExpectedDate = tripExpectedDate ?? TripExpectedDate;
        SetUpdateAt(DateTime.UtcNow);
    }
}
