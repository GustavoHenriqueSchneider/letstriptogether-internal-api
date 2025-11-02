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

        private readonly string _culturePreference;

        private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
        {
            Architecture,
            Center,
            Education,
            Heritage,
            Historical,
            Monument,
            Museum,
            Religious
        };

        public Culture(string culturePreference)
        {
            var key = culturePreference;

            if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Invalid culture preference: {key}");
            }

            _culturePreference = key;
        }

        public override string ToString() => _culturePreference;

        public static implicit operator string(Culture s) => s._culturePreference;
    }
}
