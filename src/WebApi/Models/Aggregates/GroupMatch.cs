namespace WebApi.Models.Aggregates;

public class GroupMatch : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public Destination Destination { get; init; } = null!;

    public GroupMatch() { }
}
