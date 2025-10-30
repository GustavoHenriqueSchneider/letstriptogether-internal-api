namespace WebApi.Models.ValueObjects;

public class TripCulturePreferences
{
    public const string Library = "education.library";
    public const string Culture = "entertainment.culture";
    public const string Museum = "entertainment.museum";
    public const string Attraction = "tourism.attraction";
    public const string Sights = "tourism.sights";

    private readonly string _culturePreference;

    private static readonly IReadOnlyList<string> ValidTripCulturePreferences = new List<string>()
    {
        nameof(Library),
        nameof(Culture),
        nameof(Museum),
        nameof(Attraction),
        nameof(Sights)
    };

    public TripCulturePreferences(string culturePreference)
    {
        var key = culturePreference.ToLower();

        if (!ValidTripCulturePreferences.Any(x => x.ToLower() == key))
        {
            throw new InvalidOperationException($"Invalid culture preference: {key}");
        }

        _culturePreference = key;
    }

    public override string ToString() => _culturePreference;

    public static implicit operator string(TripCulturePreferences s) => s._culturePreference;
}
