namespace WebApi.Models.ValueObjects;

public class TripEntertainmentPreferences
{
    public const string Prefix = "entertainment.";
    
    public const string Adventure = $"{Prefix}.adventure";
    public const string Attraction = $"{Prefix}.attraction";
    public const string Park = $"{Prefix}.park";
    public const string Sports = $"{Prefix}.sports";
    public const string Tour = $"{Prefix}.tour";

    private readonly string _entertainmentPreference;

    private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
    {
        nameof(Adventure),
        nameof(Attraction),
        nameof(Park),
        nameof(Sports),
        nameof(Tour)
    };

    public TripEntertainmentPreferences(string entertainmentPreference)
    {
        var key = entertainmentPreference;

        if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Invalid entertainment preference: {key}");
        }

        _entertainmentPreference = key;
    }

    public override string ToString() => _entertainmentPreference;

    public static implicit operator string(TripEntertainmentPreferences s) => s._entertainmentPreference;
}
