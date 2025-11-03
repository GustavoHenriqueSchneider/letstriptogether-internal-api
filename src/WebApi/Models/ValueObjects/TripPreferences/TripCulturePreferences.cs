namespace WebApi.Models.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string CulturePrefix = "culture";
    
    public class Culture
    {
        public const string Architecture = $"{CulturePrefix}.architecture";
        public const string Center = $"{CulturePrefix}.center";
        public const string Education = $"{CulturePrefix}.education";
        public const string Heritage = $"{CulturePrefix}.heritage";
        public const string Historical = $"{CulturePrefix}.historical";
        public const string Monument = $"{CulturePrefix}.monument";
        public const string Museum = $"{CulturePrefix}.museum";
        public const string Religious = $"{CulturePrefix}.religious";

        private readonly string _preference;

        private static readonly IReadOnlyDictionary<string,string> ValidPreferences = new Dictionary<string,string>
        {
            { nameof(Architecture), Architecture },
            { nameof(Center), Center },
            { nameof(Education), Education },
            { nameof(Heritage), Heritage },
            { nameof(Historical), Historical },
            { nameof(Monument), Monument },
            { nameof(Museum), Museum },
            { nameof(Religious), Religious }
        };

        public Culture(string preferenceValue)
        {
            var value = preferenceValue;
            
            if (!value.StartsWith($"{CulturePrefix}.", StringComparison.OrdinalIgnoreCase))
            {
                value = $"{CulturePrefix}.{preferenceValue}";
            }
            
            var preference = ValidPreferences.SingleOrDefault(x => 
                x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            _preference = preference.Key?.ToLower()
                          ?? throw new InvalidOperationException($"Invalid culture preference: {preferenceValue}");
        }

        public override string ToString() => _preference;

        public static implicit operator string(Culture s) => s._preference;
    }
}
