namespace LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;

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

        private readonly string _preference;

        private static readonly IReadOnlyDictionary<string,string> ValidPreferences = new Dictionary<string,string>
        {
            { nameof(Beach), Beach },
            { nameof(Cave), Cave },
            { nameof(Mountain), Mountain },
            { nameof(Nature), Nature },
            { nameof(Park), Park },
            { nameof(Rural), Rural },
            { nameof(Trail), Trail },
            { nameof(Viewpoint), Viewpoint },
            { nameof(Waterfall), Waterfall }
        };

        public PlaceType(string preferenceValue)
        {
            var value = preferenceValue;
            
            if (!value.StartsWith($"{PlaceTypePrefix}.", StringComparison.OrdinalIgnoreCase))
            {
                value = $"{PlaceTypePrefix}.{preferenceValue}";
            }
            
            var preference = ValidPreferences.SingleOrDefault(x => 
                x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            _preference = preference.Key?.ToLower()
                          ?? throw new InvalidOperationException($"Invalid place type preference: {preferenceValue}");
        }

        public override string ToString() => _preference;

        public static implicit operator string(PlaceType s) => s._preference;
    }
}
