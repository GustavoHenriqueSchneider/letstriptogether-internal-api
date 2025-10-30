using System.Text.RegularExpressions;

namespace WebApi.Models.ValueObjects;

public class TripPlaceTypes
{
    public const string Beach = "beach";
    public const string Camping = "camping";
    public const string Park = "leisure.park";
    public const string Mountain = "natural.mountain";

    private readonly string _placeType;

    private static readonly IReadOnlyList<string> ValidTripPlaceTypes = new List<string>()
    {
        nameof(Beach),
        nameof(Camping),
        nameof(Park),
        nameof(Mountain)
    };

    public TripPlaceTypes(string placeType)
    {
        var key = placeType.ToLower();

        if (!ValidTripPlaceTypes.Any(x => x.ToLower() == key))
        {
            throw new InvalidOperationException($"Invalid place key: {key}");
        }

        _placeType = key;
    }

    public override string ToString() => _placeType;

    public static implicit operator string(TripPlaceTypes s) => s._placeType;
}
