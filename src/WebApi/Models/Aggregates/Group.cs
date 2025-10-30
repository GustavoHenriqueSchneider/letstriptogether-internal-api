namespace WebApi.Models.Aggregates;

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

    public GroupPreference Preferences { get; private set; } = new();

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
            throw new InvalidOperationException("This member is already included on the group.");
        }

        _members.Add(member);
    }

    public void RemoveMember(GroupMember member)
    {
        if (!HasMember(member))
        {
            return;
        }

        _members.Remove(member);
        UpdatePreferences();
    }

    public GroupPreference UpdatePreferences()
    {
        Preferences.GroupId = Id;

        var foodPreferences = new List<string>();
        var culturePreferences = new List<string>();
        var entertainmentPreferences = new List<string>();
        var placeTypes = new List<string>();

        var anyMemberLikesCommercial = _members.Any(x => x.User.Preferences!.LikesCommercial == true);

        foreach (var member in _members)
        {
            var userPreference = member.User.Preferences;

            foodPreferences.AddRange(userPreference!.Food);
            culturePreferences.AddRange(userPreference.Culture);
            entertainmentPreferences.AddRange(userPreference.Entertainment);
            placeTypes.AddRange(userPreference.PlaceTypes);
        }

        var newPreferences = new GroupPreference(anyMemberLikesCommercial, foodPreferences.ToHashSet(),
            culturePreferences.ToHashSet(), entertainmentPreferences.ToHashSet(), placeTypes.ToHashSet());

        Preferences.Update(newPreferences);

        return Preferences;
    }

    public GroupPreference UpdatePreferences(GroupPreference groupPreference)
    {
        Preferences.GroupId = Id;
        Preferences.Update(groupPreference);

        return Preferences;
    }

    public void Update(string? name, DateTime? tripExpectedDate)
    {
        Name = name ?? Name;
        TripExpectedDate = tripExpectedDate ?? TripExpectedDate;
    }
}
