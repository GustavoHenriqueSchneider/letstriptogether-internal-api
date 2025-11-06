using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;

public class User : TrackableEntity
{
    public const int NameMaxLength = 150;
    public const int EmailMaxLength = 254;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 30;
    
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsAnonymous { get; private set; }

    private readonly List<GroupMember> _groupMemberships = [];
    public IReadOnlyCollection<GroupMember> GroupMemberships => _groupMemberships.AsReadOnly();
    
    private readonly List<UserGroupInvitation> _acceptedInvitations = [];
    public IReadOnlyCollection<UserGroupInvitation> AcceptedInvitations => _acceptedInvitations.AsReadOnly();
    
    public UserPreference? Preferences { get; private set; }

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
    }

    public void SetPreferences(UserPreference preferences)
    {
        preferences.UserId = Id;

        if (Preferences is null)
        {
            Preferences = preferences;
        }
        else
        {
            Preferences.Update(preferences);
        }
    }

    public void Anonymize()
    {
        Name = "AnonymousUser";
        Email = $"anon_{Guid.NewGuid():N}@deleted.local";
        IsAnonymous = true;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    private void AddRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
        {
            return;
        }

        _userRoles.Add(new UserRole(Id, role));
    }
}
