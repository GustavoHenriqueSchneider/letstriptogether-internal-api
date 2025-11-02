namespace WebApi.Models.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string PlaceTypePrefix = "placetype";
    
    public class PlaceType
    {
        public const string Beach = $"{PlaceTypePrefix}.beach";
        public const string Cave = $"{PlaceTypePrefix}.cave";
        public const string Mountain = $"{PlaceTypePrefix}.mountain";
        public const string Nature = $"{PlaceTypePrefix}.nature";
        public const string Park = $"{PlaceTypePrefix}.park";
        public const string Rural = $"{PlaceTypePrefix}.rural";
        public const string Trail = $"{PlaceTypePrefix}.trail";
        public const string Viewpoint = $"{PlaceTypePrefix}.viewpoint";
        public const string Waterfall = $"{PlaceTypePrefix}.waterfall";

        private readonly string _placeType;

        private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
        {
            Beach,
            Cave,
            Mountain,
            Nature,
            Park,
            Rural,
            Trail,
            Viewpoint,
            Waterfall
        };

        public PlaceType(string placeType)
        {
            var key = placeType;

            if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Invalid place type: {key}");
            }

            _placeType = key;
        }

        public override string ToString() => _placeType;

        public static implicit operator string(PlaceType s) => s._placeType;
    }
}
