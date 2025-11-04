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

    private bool HasMember(GroupMember member)
    {
        return _members.Contains(member);
    }

    public GroupMember AddMember(User user, bool isOwner)
    {
        var member = new GroupMember
        {
            GroupId = Id,
            UserId = user.Id,
            IsOwner = isOwner
        };
        
        if (HasMember(member))
        {
            throw new InvalidOperationException("This member is already included on the group.");
        }

        _members.Add(member);

        if (!AllMembersHavePreferences())
        {
            return member;
        }
        
        UpdatePreferences();
        return member;
    }

    private bool AllMembersHavePreferences()
    {
        return _members.All(m => m.User?.Preferences is not null);
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
    
    private bool HasMatch(GroupMatch match)
    {
        return _matches.Contains(match);
    }

    public GroupMatch CreateMatch(Guid destinationId)
    {
        if (_members.Count <= 1)
        {
            throw new InvalidOperationException("It is not possible to create a group match with only one member.");
        }

        var membersAgree = _members.All(member =>
        {
            var vote = member.Votes.SingleOrDefault(vote => vote.DestinationId == destinationId);
            return vote is not null && vote.IsApproved;
        });

        if (!membersAgree)
        {
            throw new InvalidOperationException("Not all group members agreed with the informed destination.");
        }

        var match = new GroupMatch
        {
            GroupId = Id,
            DestinationId = destinationId
        };

        if (HasMatch(match))
        {
            throw new InvalidOperationException("This match is already included on the group.");
        }
        
        _matches.Add(match);
        return match;
    }

    public GroupPreference UpdatePreferences()
    {
        Preferences.GroupId = Id;

        var foodPreferences = new List<string>();
        var culturePreferences = new List<string>();
        var entertainmentPreferences = new List<string>();
        var placeTypes = new List<string>();

        var anyMemberLikesCommercial = _members.Any(x => x.User.Preferences!.LikesCommercial);

        foreach (var userPreference in _members.Select(member => member.User.Preferences))
        {
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
