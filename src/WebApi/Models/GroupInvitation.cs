namespace WebApi.Models;

public class GroupInvitation : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;

    private DateTime _expirationDate;
    public DateTime ExpirationDate
    {
        get => _expirationDate.ToUniversalTime();
        init => _expirationDate = value;
    }

    private readonly List<UserGroupInvitation> _answeredBy = [];
    public IReadOnlyCollection<UserGroupInvitation> AnsweredBy => _answeredBy.AsReadOnly();

    private GroupInvitation() { }
}