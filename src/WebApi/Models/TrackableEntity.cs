namespace WebApi.Models;

public class TrackableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    private DateTime _createdAt;
    public DateTime CreatedAt 
    {
        get => _createdAt.ToUniversalTime();
        init => _createdAt = DateTime.UtcNow;
    }

    private DateTime? _updateAt;
    public DateTime? UpdatedAt
    {
        get => _updateAt?.ToUniversalTime();
        private set => _updateAt = value ?? null;
    }

    protected void SetUpdateAt(DateTime updatedAt) 
    {  
        UpdatedAt = updatedAt;
    }
}
