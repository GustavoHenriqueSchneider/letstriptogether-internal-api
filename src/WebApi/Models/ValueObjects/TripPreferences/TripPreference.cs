namespace WebApi.Models.ValueObjects.TripPreferences;

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
            throw new InvalidOperationException($"Invalid preference for {nameof(TripPreference)}");
        }

        _category = prefix switch
        {
            CulturePrefix => new Culture(preference),
            EntertainmentPrefix => new Entertainment(preference),
            FoodPrefix => new Food(preference),
            PlaceTypePrefix => new PlaceType(preference),
            _ => throw new InvalidOperationException($"Invalid prefix {prefix} for preference")
        };
    }
    
    public override string ToString() => _category;

    public static implicit operator string(TripPreference s) => s._category;
}