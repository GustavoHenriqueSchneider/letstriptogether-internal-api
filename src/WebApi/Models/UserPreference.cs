namespace WebApi.Models;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    // TODO: fazer listas readonly
    public List<string> Categories { get; init; } = [];

    public void Update(UserPreference preferences)
    {
        Categories.Clear();
        Categories.AddRange(preferences.Categories);
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }
}