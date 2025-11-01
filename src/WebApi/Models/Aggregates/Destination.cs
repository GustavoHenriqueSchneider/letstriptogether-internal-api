using WebApi.Models.ValueObjects;

namespace WebApi.Models.Aggregates;

public class Destination : TrackableEntity
{
    private const string ShoppingCategory = "shopping";
    
    public string Address { get; init; } = null!;
    public string Description { get; init; } = null!;

    private readonly List<GroupMatch> _groupMatches = [];
    public IReadOnlyCollection<GroupMatch> GroupMatches => _groupMatches.AsReadOnly();

    private readonly List<GroupMemberDestinationVote> _groupMemberVotes = [];
    public IReadOnlyCollection<GroupMemberDestinationVote> GroupMemberVotes => _groupMemberVotes.AsReadOnly();

    private readonly List<DestinationAttraction> _attractions = [];
    public IReadOnlyCollection<DestinationAttraction> Attractions => _attractions.AsReadOnly();
    
    private Destination() { }

    public bool HasCommercialCategory() => 
        _attractions.Any(a => a.Category.Equals(ShoppingCategory, StringComparison.OrdinalIgnoreCase));
    
    public IReadOnlyCollection<string> GetFoodCategories() => 
        GetReadonlyCategoryList(_attractions, TripFoodPreferences.Prefix);
    
    public IReadOnlyCollection<string> GetCultureCategories() => 
        GetReadonlyCategoryList(_attractions, TripCulturePreferences.Prefix);
    
    public IReadOnlyCollection<string> GetEntertainmentCategories() => 
        GetReadonlyCategoryList(_attractions, TripEntertainmentPreferences.Prefix);
    
    public IReadOnlyCollection<string> GetPlaceTypes() => 
        GetReadonlyCategoryList(_attractions, TripPlaceTypes.Prefix);

    private static IReadOnlyCollection<string> GetReadonlyCategoryList(List<DestinationAttraction> list, string prefix)
    {
        return list
            .Where(a => a.Category.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(a => a.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList()
            .AsReadOnly();
    }
}
