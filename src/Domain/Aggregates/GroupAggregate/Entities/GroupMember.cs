using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

public class GroupMember : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public bool IsOwner { get; init; }
    
    private readonly List<GroupMemberDestinationVote> _votes = [];
    public IReadOnlyCollection<GroupMemberDestinationVote> Votes => _votes.AsReadOnly();
}
