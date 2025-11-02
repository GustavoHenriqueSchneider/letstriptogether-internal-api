namespace WebApi.Models.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string EntertainmentPrefix = "entertainment";
    
    public class Entertainment
    {
        public const string Adventure = $"{EntertainmentPrefix}.adventure";
        public const string Attraction = $"{EntertainmentPrefix}.attraction";
        public const string Park = $"{EntertainmentPrefix}.park";
        public const string Sports = $"{EntertainmentPrefix}.sports";
        public const string Tour = $"{EntertainmentPrefix}.tour";

        private readonly string _entertainmentPreference;

        private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
        {
            Adventure,
            Attraction,
            Park,
            Sports,
            Tour
        };

        public Entertainment(string entertainmentPreference)
        {
            var key = entertainmentPreference;

            if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Invalid entertainment preference: {key}");
            }

            _entertainmentPreference = key;
        }

        public override string ToString() => _entertainmentPreference;

        public static implicit operator string(Entertainment s) => s._entertainmentPreference;
    }
}
