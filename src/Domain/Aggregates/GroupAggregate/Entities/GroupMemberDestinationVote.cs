using Domain.Aggregates.DestinationAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.GroupAggregate.Entities;

public class GroupMemberDestinationVote : TrackableEntity
{
    public Guid GroupMemberId { get; init; }
    public GroupMember GroupMember { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public Destination Destination { get; init; } = null!;
    public bool IsApproved { get; private set; }

    private GroupMemberDestinationVote() { }

    public GroupMemberDestinationVote(Guid groupMemberId, Guid destinationId, bool isApproved)
    {
        GroupMemberId = groupMemberId;
        DestinationId = destinationId;
        IsApproved = isApproved;
    }

    public void SetApproved(bool isApproved)
    {
        IsApproved = isApproved;
    }
}
