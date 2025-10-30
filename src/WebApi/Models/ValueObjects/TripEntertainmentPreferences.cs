using System.Text.RegularExpressions;

namespace WebApi.Models.ValueObjects;

public class TripEntertainmentPreferences
{
    public const string Cinema = "entertainment.cinema";
    public const string ActivityPark = "entertainment.activity_park";
    public const string Zoo = "entertainment.zoo";
    public const string Spa = "leisure.spa";
    public const string Sport = "sport";

    private readonly string _entertainmentPreference;

    private static readonly IReadOnlyList<string> ValidTripEntertainmentPreferences = new List<string>()
    {
        nameof(Cinema),
        nameof(ActivityPark),
        nameof(Zoo),
        nameof(Spa),
        nameof(Sport)
    };

    public TripEntertainmentPreferences(string entertainmentPreference)
    {
        var key = entertainmentPreference.ToLower();

        if (!ValidTripEntertainmentPreferences.Any(x => x.ToLower() == key))
        {
            throw new InvalidOperationException($"Invalid entertainment preference: {key}");
        }

        _entertainmentPreference = key;
    }

    public override string ToString() => _entertainmentPreference;

    public static implicit operator string(TripEntertainmentPreferences s) => s._entertainmentPreference;
}
