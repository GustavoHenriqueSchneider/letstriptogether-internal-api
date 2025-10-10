namespace WebApi.Models;

public class TrackableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void SetUpdateAt(DateTime updatedAt) 
    {  
        UpdatedAt = updatedAt;
    }
}
