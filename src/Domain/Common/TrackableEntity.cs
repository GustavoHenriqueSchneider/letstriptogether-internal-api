namespace Domain.Common;

public class TrackableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    private DateTime _createdAt = DateTime.UtcNow;
    public DateTime CreatedAt
    {
        get => _createdAt.ToUniversalTime();
        private set => _createdAt = value;
    }

    private DateTime? _updatedAt;
    public DateTime? UpdatedAt
    {
        get => _updatedAt?.ToUniversalTime();
        private set =>_updatedAt = value;
    }

    public void SetUpdateAt()
    {
        _updatedAt = DateTime.UtcNow;
    }
}
