namespace WebApi.Models.ValueObjects;

public class TripCulturePreferences
{
    public const string Prefix = "culture.";
    
    public const string Architecture = $"{Prefix}.architecture";
    public const string Center = $"{Prefix}.center";
    public const string Education = $"{Prefix}.education";
    public const string Heritage = $"{Prefix}.heritage";
    public const string Historical = $"{Prefix}.historical";
    public const string Monument = $"{Prefix}.monument";
    public const string Museum = $"{Prefix}.museum";
    public const string Religious = $"{Prefix}.religious";

    private readonly string _culturePreference;

    private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
    {
        nameof(Architecture),
        nameof(Center),
        nameof(Education),
        nameof(Heritage),
        nameof(Historical),
        nameof(Monument),
        nameof(Museum),
        nameof(Religious)
    };

    public TripCulturePreferences(string culturePreference)
    {
        var key = culturePreference;

        if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Invalid culture preference: {key}");
        }

        _culturePreference = key;
    }

    public override string ToString() => _culturePreference;

    public static implicit operator string(TripCulturePreferences s) => s._culturePreference;
}
