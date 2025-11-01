namespace WebApi.Models.ValueObjects;

public class TripFoodPreferences
{
    public const string Prefix = "food.";
    
    public const string Restaurant = $"{Prefix}.restaurant";

    private readonly string _foodPreference;

    private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
    {
        nameof(Restaurant)
    };

    public TripFoodPreferences(string foodPreference)
    {
        var key = foodPreference;

        if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Invalid food preference: {key}");
        }

        _foodPreference = key;
    }

    public override string ToString() => _foodPreference;

    public static implicit operator string(TripFoodPreferences s) => s._foodPreference;
}
