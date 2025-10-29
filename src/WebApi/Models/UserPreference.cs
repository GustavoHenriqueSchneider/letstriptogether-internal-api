namespace WebApi.Models;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;

    private readonly List<string> _categories = [];
    public IReadOnlyCollection<string> Categories => _categories.AsReadOnly();

    private UserPreference() { }

    public UserPreference(List<string> categories)
    {
        _categories = categories;
    }

    public void Update(UserPreference preferences)
    {
        _categories.Clear();
        _categories.AddRange(preferences.Categories);
        // TODO: setupdatedat vai ir pra override do update ou algo assim no repository/unitofwork
        SetUpdateAt(DateTime.UtcNow);
    }
}