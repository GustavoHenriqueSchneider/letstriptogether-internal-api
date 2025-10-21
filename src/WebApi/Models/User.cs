namespace WebApi.Models;

public class User : TrackableEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsAnonymous { get; private set; }

    private readonly List<GroupMember> _groupMemberships = [];
    public IReadOnlyCollection<GroupMember> GroupMemberships => _groupMemberships.AsReadOnly();
    
    private readonly List<UserGroupInvitation> _acceptedInvitations = [];
    public IReadOnlyCollection<UserGroupInvitation> AcceptedInvitations => _acceptedInvitations.AsReadOnly();
    
    public UserPreference Preferences { get; private set; } = new([]);

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { }

    public User(string name, string email, string passwordHash, Role role)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        AddRole(role);
    }

    public void Update(string? name)
    {
        Name = name ?? Name;
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void SetPreferences(UserPreference preferences)
    {
        Preferences.Update(preferences);
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void Anonymize()
    {
        Name = "AnonymousUser";
        Email = $"anon_{Guid.NewGuid():N}@deleted.local";
        IsAnonymous = true;
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void AddRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
        {
            return;
        }

        _userRoles.Add(new UserRole(Id, role.Id));
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void RemoveRole(Guid roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);

        if (userRole is null)
        {
            return;
        }

        _userRoles.Remove(userRole);
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }
}
