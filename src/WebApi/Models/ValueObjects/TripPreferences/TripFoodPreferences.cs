namespace WebApi.Models.ValueObjects.TripPreferences;

public partial class TripPreference
{
    public const string FoodPrefix = "food";
    
    public class Food
    {
        public const string Restaurant = $"{FoodPrefix}.restaurant";

        private readonly string _foodPreference;

        private static readonly IReadOnlyList<string> ValidPreferences = new List<string>
        {
            Restaurant
        };

        public Food(string foodPreference)
        {
            var key = foodPreference;

            if (!ValidPreferences.Any(v => v.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Invalid food preference: {key}");
            }

            _foodPreference = key;
        }

        public override string ToString() => _foodPreference;

        public static implicit operator string(Food s) => s._foodPreference;
    }
}
