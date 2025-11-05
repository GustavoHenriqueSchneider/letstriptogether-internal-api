using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; set; }
    public User User { get; init; } = null!;
    public bool LikesCommercial { get; private set; }

    private readonly List<string> _food = [];
    public IReadOnlyCollection<string> Food => _food.AsReadOnly();

    private readonly List<string> _culture = [];
    public IReadOnlyCollection<string> Culture => _culture.AsReadOnly();

    private readonly List<string> _entertainment = [];
    public IReadOnlyCollection<string> Entertainment => _entertainment.AsReadOnly();

    private readonly List<string> _placeTypes = [];
    public IReadOnlyCollection<string> PlaceTypes => _placeTypes.AsReadOnly();

    public UserPreference() { }

    public UserPreference(UserPreference? userPreference)
    {
        if (userPreference is null)
        {
            return;
        }

        LikesCommercial = userPreference.LikesCommercial;
        userPreference.Food.ToList().ForEach(x => _food.Add(new TripPreference.Food(x)));
        userPreference.Culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        userPreference.Entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        userPreference.PlaceTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public UserPreference(bool likesCommercial, List<string> food,
        List<string> culture, List<string> entertainment, List<string> placeTypes)
    {
        LikesCommercial = likesCommercial;
        food.ForEach(x => _food.Add(new TripPreference.Food(x)));
        culture.ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        entertainment.ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        placeTypes.ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public void Update(UserPreference userPreference)
    {
        LikesCommercial = userPreference.LikesCommercial;

        _food.Clear();
        _food.AddRange(userPreference.Food);

        _culture.Clear();
        _culture.AddRange(userPreference.Culture);

        _entertainment.Clear();
        _entertainment.AddRange(userPreference.Entertainment);

        _placeTypes.Clear();
        _placeTypes.AddRange(userPreference.PlaceTypes);
    }

    public static implicit operator GroupPreference(UserPreference userPreference)
    {
        return new GroupPreference(userPreference.LikesCommercial, userPreference.Food,
            userPreference.Culture, userPreference.Entertainment, userPreference.PlaceTypes);
    }
}
