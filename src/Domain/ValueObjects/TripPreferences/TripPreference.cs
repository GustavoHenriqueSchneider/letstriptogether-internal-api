using Domain.Common.Exceptions;

namespace Domain.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string Shopping = "shopping";

    private readonly string _category;
    
    public TripPreference(string preference)
    {
        if (preference == Shopping)
        {
            _category = preference;
            return;
        }
        
        var prefix = preference.Split('.').FirstOrDefault()?.ToLower();

        if (prefix is null)
        {
            throw new DomainBusinessRuleException($"Invalid preference for {nameof(TripPreference)}");
        }

        _category = prefix switch
        {
            CulturePrefix => new Culture(preference),
            EntertainmentPrefix => new Entertainment(preference),
            FoodPrefix => new Food(preference),
            PlaceTypePrefix => new PlaceType(preference),
            _ => throw new DomainBusinessRuleException($"Invalid prefix {prefix} for preference")
        };

        _category = _category.ToLower();
    }
    
    public override string ToString() => _category;

    public static implicit operator string(TripPreference s) => s._category;
}