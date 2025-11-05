using LetsTripTogether.InternalApi.Domain.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;

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

        private readonly string _preference;

        private static readonly IReadOnlyDictionary<string,string> ValidPreferences = new Dictionary<string,string>
        {
            { nameof(Adventure), Adventure },
            { nameof(Attraction), Attraction },
            { nameof(Park), Park },
            { nameof(Sports), Sports },
            { nameof(Tour), Tour }
        };

        public Entertainment(string preferenceValue)
        {
            var value = preferenceValue;
            
            if (!value.StartsWith($"{EntertainmentPrefix}.", StringComparison.OrdinalIgnoreCase))
            {
                value = $"{EntertainmentPrefix}.{preferenceValue}";
            }
            
            var preference = ValidPreferences.SingleOrDefault(x => 
                x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            _preference = preference.Key?.ToLower()
                          ?? throw new DomainBusinessRuleException($"Invalid entertainment preference: {preferenceValue}");
        }

        public override string ToString() => _preference;

        public static implicit operator string(Entertainment s) => s._preference;
    }
}
