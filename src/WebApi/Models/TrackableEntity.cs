using System.Text.Json.Serialization;

namespace WebApi.Models;

public class TrackableEntity
{
    [JsonIgnore] public Guid Id { get; init; } = Guid.NewGuid();
    [JsonIgnore] public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    [JsonIgnore] public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdateAt(DateTime updatedAt) 
    {  
        UpdatedAt = updatedAt;
    }
}
