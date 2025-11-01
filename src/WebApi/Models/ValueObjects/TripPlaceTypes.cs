namespace WebApi.Models.ValueObjects;

public class TripPlaceTypes
{
    public const string Prefix = "placetype.";
    
    public const string Beach = $"{Prefix}.beach";
    public const string Cave = $"{Prefix}.cave";
    public const string Mountain = $"{Prefix}.mountain";
    public const string Nature = $"{Prefix}.nature";
    public const string Park = $"{Prefix}.park";
    public const string Rural = $"{Prefix}.rural";
    public const string Trail = $"{Prefix}.trail";
    public const string Viewpoint = $"{Prefix}.viewpoint";
    public const string Waterfall = $"{Prefix}.waterfall";

    private readonly string _placeType;

    private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
    {
        nameof(Beach),
        nameof(Cave),
        nameof(Mountain),
        nameof(Nature),
        nameof(Park),
        nameof(Rural),
        nameof(Trail),
        nameof(Viewpoint),
        nameof(Waterfall)
    };

    public TripPlaceTypes(string placeType)
    {
        var key = placeType;

        if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Invalid place key: {key}");
        }

        _placeType = key;
    }

    public override string ToString() => _placeType;

    public static implicit operator string(TripPlaceTypes s) => s._placeType;
}
