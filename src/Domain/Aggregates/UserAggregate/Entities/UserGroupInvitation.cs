using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.UserAggregate.Entities;

public class UserGroupInvitation : TrackableEntity
{
    public Guid GroupInvitationId { get; init; }
    public GroupInvitation GroupInvitation { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public bool IsAccepted { get; init; }

    public UserGroupInvitation() { }
}
