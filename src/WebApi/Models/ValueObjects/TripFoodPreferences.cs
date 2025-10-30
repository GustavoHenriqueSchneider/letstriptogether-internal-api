using System.Text.RegularExpressions;

namespace WebApi.Models.ValueObjects;

public class TripFoodPreferences
{
    public const string Bar = "catering.bar";
    public const string Cafe = "catering.cafe";
    public const string FastFood = "catering.fast_food";
    public const string Restaurant = "catering.restaurant";

    private readonly string _foodPreference;

    private static readonly IReadOnlyList<string> ValidTripFoodPreferences = new List<string>()
    {
        nameof(Bar),
        nameof(Cafe),
        nameof(FastFood),
        nameof(Restaurant)
    };

    public TripFoodPreferences(string foodPreference)
    {
        var key = foodPreference.ToLower();

        if (!ValidTripFoodPreferences.Any(x => x.ToLower() == key))
        {
            throw new InvalidOperationException($"Invalid food preference: {key}");
        }

        _foodPreference = key;
    }

    public override string ToString() => _foodPreference;

    public static implicit operator string(TripFoodPreferences s) => s._foodPreference;
}
