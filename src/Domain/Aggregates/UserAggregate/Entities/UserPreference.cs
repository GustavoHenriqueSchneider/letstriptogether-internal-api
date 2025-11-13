using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;
using Domain.ValueObjects.TripPreferences;

namespace Domain.Aggregates.UserAggregate.Entities;

public class UserPreference : TrackableEntity
{
    public Guid UserId { get; set; }
    public User User { get; init; } = null!;
    public bool LikesShopping { get; private set; }
    public bool LikesGastronomy { get; private set; }

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

        LikesShopping = userPreference.LikesShopping;
        LikesGastronomy = userPreference.LikesGastronomy;
        userPreference.Culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        userPreference.Entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        userPreference.PlaceTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public UserPreference(bool likesShopping, bool likesGastronomy,
        List<string> culture, List<string> entertainment, List<string> placeTypes)
    {
        LikesShopping = likesShopping;
        LikesGastronomy = likesGastronomy;
        culture.ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        entertainment.ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        placeTypes.ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public void Update(UserPreference userPreference)
    {
        LikesShopping = userPreference.LikesShopping;
        LikesGastronomy = userPreference.LikesGastronomy;

        _culture.Clear();
        _culture.AddRange(userPreference.Culture);

        _entertainment.Clear();
        _entertainment.AddRange(userPreference.Entertainment);

        _placeTypes.Clear();
        _placeTypes.AddRange(userPreference.PlaceTypes);
    }

    public static implicit operator GroupPreference(UserPreference userPreference)
    {
        return new GroupPreference(userPreference.LikesShopping, userPreference.LikesGastronomy,
            userPreference.Culture, userPreference.Entertainment, userPreference.PlaceTypes);
    }
}
