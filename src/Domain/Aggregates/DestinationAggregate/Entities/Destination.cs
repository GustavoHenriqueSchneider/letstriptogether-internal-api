using System.Collections.ObjectModel;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;
using Domain.ValueObjects.TripPreferences;

namespace Domain.Aggregates.DestinationAggregate.Entities;

public class Destination : TrackableEntity
{
    public string Address { get; init; } = null!;
    public string Description { get; init; } = null!;

    private readonly List<GroupMatch> _groupMatches = [];
    public IReadOnlyCollection<GroupMatch> GroupMatches => _groupMatches.AsReadOnly();

    private readonly List<GroupMemberDestinationVote> _groupMemberVotes = [];
    public IReadOnlyCollection<GroupMemberDestinationVote> GroupMemberVotes => _groupMemberVotes.AsReadOnly();

    private readonly List<DestinationAttraction> _attractions = [];
    public IReadOnlyCollection<DestinationAttraction> Attractions => _attractions.AsReadOnly();
    
    public Destination() { }

    public bool HasShoppingCategory() => _attractions.Any(a => 
        a.Category.Equals(TripPreference.Shopping, StringComparison.OrdinalIgnoreCase));
    
    public bool HasGastronomyCategory() => _attractions.Any(a => 
        a.Category.Equals(TripPreference.Gastronomy, StringComparison.OrdinalIgnoreCase));
    
    public IReadOnlyCollection<string> GetCultureCategories() => 
        GetReadonlyCategoryList(_attractions, TripPreference.CulturePrefix);
    
    public IReadOnlyCollection<string> GetEntertainmentCategories() => 
        GetReadonlyCategoryList(_attractions, TripPreference.EntertainmentPrefix);
    
    public IReadOnlyCollection<string> GetPlaceTypes() => 
        GetReadonlyCategoryList(_attractions, TripPreference.PlaceTypePrefix);

    private static ReadOnlyCollection<string> GetReadonlyCategoryList(List<DestinationAttraction> list, string prefix)
    {
        return list
            .Where(a =>
            {
                var categoryPrefix = a.Category.Split('.').First();
                return categoryPrefix.Equals(prefix, StringComparison.OrdinalIgnoreCase);
            })
            .Select(a => a.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList()
            .AsReadOnly();
    }
}
