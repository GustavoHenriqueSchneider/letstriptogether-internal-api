using Domain.Common;
using Domain.ValueObjects.TripPreferences;

namespace Domain.Aggregates.GroupAggregate.Entities;

public class GroupPreference : TrackableEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; init; } = null!;
    public bool LikesCommercial { get; private set; }

    private readonly List<string> _food = [];
    public IReadOnlyCollection<string> Food => _food.AsReadOnly();

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

        LikesCommercial = groupPreference.LikesCommercial;
        groupPreference.Food.ToList().ForEach(x => _food.Add(new TripPreference.Food(x)));
        groupPreference.Culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        groupPreference.Entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        groupPreference.PlaceTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public GroupPreference(bool likesCommercial, IEnumerable<string> food,
        IEnumerable<string> culture, IEnumerable<string> entertainment, IEnumerable<string> placeTypes)
    {
        LikesCommercial = likesCommercial;
        food.ToList().ForEach(x => _food.Add(new TripPreference.Food(x)));
        culture.ToList().ForEach(x => _culture.Add(new TripPreference.Culture(x)));
        entertainment.ToList().ForEach(x => _entertainment.Add(new TripPreference.Entertainment(x)));
        placeTypes.ToList().ForEach(x => _placeTypes.Add(new TripPreference.PlaceType(x)));
    }

    public void Update(GroupPreference groupPreference)
    {
        LikesCommercial = groupPreference.LikesCommercial;

        _food.Clear();
        _food.AddRange(groupPreference.Food);

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

        if (LikesCommercial)
        {
            list.Add(TripPreference.Shopping.ToLower());
        }
        
        list.AddRange(_food.Select(x => $"{TripPreference.FoodPrefix}.{x}".ToLower()));
        list.AddRange(_culture.Select(x => $"{TripPreference.CulturePrefix}.{x}".ToLower()));
        list.AddRange(_entertainment.Select(x => $"{TripPreference.EntertainmentPrefix}.{x}".ToLower()));
        list.AddRange(_placeTypes.Select(x => $"{TripPreference.PlaceTypePrefix}.{x}".ToLower()));

        return list.Distinct().ToList().AsReadOnly();
    }
}
