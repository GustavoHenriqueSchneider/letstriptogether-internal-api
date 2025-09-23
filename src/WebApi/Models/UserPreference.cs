namespace WebApi.Models;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public List<string> Categories { get; set; } = [];

    public void Update(UserPreference preferences)
    {
        Categories = preferences.Categories;
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }
}
