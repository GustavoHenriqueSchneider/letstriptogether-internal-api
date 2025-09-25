using System.Text.Json.Serialization;

namespace WebApi.Models;

public class User : TrackableEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool IsAnonymous { get; private set; }

    // TODO: expor listas como readonly e criar metodos para incluir dados
    public List<GroupMember> GroupMemberships { get; private set; } = [];
    public List<UserGroupInvitation> AcceptedInvitations { get; private set; } = [];
    public UserPreference Preferences { get; private set; } = new();

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
<<<<<<< Updated upstream
=======
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }

    public void Anonymize()
    {
        // TODO: melhorar logica de anonimização
        Name = "AnonymousUser";
        Email = $"anon_{Guid.NewGuid():N}@deleted.local";
        IsAnonymous = true;
        
        Preferences.Categories = new List<string>();
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
>>>>>>> Stashed changes
        SetUpdateAt(DateTime.UtcNow);
    }
}
