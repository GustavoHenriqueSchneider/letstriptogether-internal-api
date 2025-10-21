namespace WebApi.Models;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    // TODO: fazer listas readonly
    public List<string> Categories { get; set; } = [];

    private UserPreference() { }

    public UserPreference(List<string> categories)
    {
        Categories = categories;
    }

    public void Update(UserPreference preferences)
    {
        Categories = preferences.Categories;
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }
}
