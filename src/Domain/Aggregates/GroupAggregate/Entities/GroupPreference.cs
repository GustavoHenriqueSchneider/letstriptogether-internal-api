using Domain.Common;
using Domain.ValueObjects.TripPreferences;

namespace Domain.Aggregates.GroupAggregate.Entities;

public class GroupPreference : TrackableEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; init; } = null!;
    public bool LikesShopping { get; private set; }
    public bool LikesGastronomy { get; private set; }

    private readonly List<string> _culture = [];
    public IReadOnlyCollection<string> Culture => _culture.AsReadOnly();

    private readonly List<string> _entertainment = [];
    public IReadOnlyCollection<string> Entertainment => _entertainment.AsReadOnly();

    private readonly List<string> _placeTypes = [];
    public IReadOnlyCollection<string> PlaceTypes => _placeTypes.AsReadOnly();

    public GroupPreference() { }

    public GroupPreference(GroupPreference? groupPreference) 
    {
        if (groupPreference is null)
        {
            return;
        }

        LikesShopping = groupPreference.LikesShopping;
        LikesGastronomy = groupPreference.LikesGastronomy;
        groupPreference.Culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        groupPreference.Entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        groupPreference.PlaceTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public GroupPreference(bool likesShopping, bool likesGastronomy,
        IEnumerable<string> culture, IEnumerable<string> entertainment, IEnumerable<string> placeTypes)
    {
        LikesShopping = likesShopping;
        LikesGastronomy = likesGastronomy;
        culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        placeTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public void Update(GroupPreference groupPreference)
    {
        LikesShopping = groupPreference.LikesShopping;
        LikesGastronomy = groupPreference.LikesGastronomy;

        _culture.Clear();
        _culture.AddRange(groupPreference.Culture);

        _entertainment.Clear();
        _entertainment.AddRange(groupPreference.Entertainment);

        _placeTypes.Clear();
        _placeTypes.AddRange(groupPreference.PlaceTypes);
    }
    
    public IReadOnlyList<string> ToList()
    {
        var list = new List<string>();

        if (LikesShopping)
        {
            list.Add(TripPreference.Shopping.ToLower());
        }
        
        if (LikesGastronomy)
        {
            list.Add(TripPreference.Gastronomy.ToLower());
        }
        
        list.AddRange(_culture.Select(x => $"{TripPreference.CulturePrefix}.{x}".ToLower()));
        list.AddRange(_entertainment.Select(x => $"{TripPreference.EntertainmentPrefix}.{x}".ToLower()));
        list.AddRange(_placeTypes.Select(x => $"{TripPreference.PlaceTypePrefix}.{x}".ToLower()));

        return list.Distinct().ToList().AsReadOnly();
    }
}
