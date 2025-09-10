using System.Text.Json.Serialization;

namespace WebApi.Models;

public class UserPreference : TrackableEntity
{
    [JsonIgnore] public Guid UserId { get; init; }
    // TODO: voltar campo abaixo para nao nullable apos aplicar dto/usecases/cqrs
    [JsonIgnore] public User? User { get; init; }
    public List<string> Categories { get; set; } = [];

    public void Update(UserPreference preferences)
    {
        Categories = preferences.Categories;
        this.SetUpdateAt(DateTime.UtcNow);
    }
}
