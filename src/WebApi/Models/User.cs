using System.Text.Json.Serialization;

namespace WebApi.Models;

public class User : TrackableEntity
{
    private string _name = null!;
    public string Name 
    { 
        get => _name; 
        init => _name = value; 
    }
    public string Email { get; init; } = null!;
    public string PasswordHash { get; init; } = null!;
    [JsonIgnore] public List<GroupMember> GroupMemberships { get; init; } = [];
    [JsonIgnore] public List<UserGroupInvitation> AcceptedInvitations { get; init; } = [];
    [JsonIgnore] public UserPreference Preferences { get; init; } = new();

    public void SetName(string name)
    {
        _name = name;
    }

    public void Update(User user)
    {
        SetName(user.Name);
        SetUpdateAt(DateTime.UtcNow);
    }

    public void SetPreferences(UserPreference preferences)
    {
        Preferences.Update(preferences);
        SetUpdateAt(DateTime.UtcNow);
    }
}
