using WebApi.Models.Enums;

namespace WebApi.Models.Aggregates;

public class GroupInvitation : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;

    private DateTime _expirationDate;
    public DateTime ExpirationDate
    {
        get => _expirationDate.ToUniversalTime();
        private set => _expirationDate = value;
    }

    public GroupInvitationStatus Status { get; private set; } = GroupInvitationStatus.Active;

    private readonly List<UserGroupInvitation> _answeredBy = [];
    public IReadOnlyCollection<UserGroupInvitation> AnsweredBy => _answeredBy.AsReadOnly();

    private GroupInvitation() { }
}