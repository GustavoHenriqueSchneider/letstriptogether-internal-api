using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;

public class UserGroupInvitation : TrackableEntity
{
    public Guid GroupInvitationId { get; init; }
    public GroupInvitation GroupInvitation { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public bool IsAccepted { get; init; }

    public UserGroupInvitation() { }
}
