namespace WebApi.Models.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string FoodPrefix = "food";
    
    public class Food
    {
        public const string Restaurant = $"{FoodPrefix}.restaurant";

        private readonly string _preference;

        private static readonly IReadOnlyDictionary<string,string> ValidPreferences = new Dictionary<string,string>
        {
            { nameof(Restaurant), Restaurant }
        };

        public Food(string preferenceValue)
        {
            var value = preferenceValue;
            
            if (!value.StartsWith($"{FoodPrefix}.", StringComparison.OrdinalIgnoreCase))
            {
                value = $"{FoodPrefix}.{preferenceValue}";
            }
            
            var preference = ValidPreferences.SingleOrDefault(x => 
                x.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            _preference = preference.Key?.ToLower()
                          ?? throw new InvalidOperationException($"Invalid food preference: {preferenceValue}");
        }

        public override string ToString() => _preference;

        public static implicit operator string(Food s) => s._preference;
    }
}
