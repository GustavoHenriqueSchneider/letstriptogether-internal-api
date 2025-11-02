namespace WebApi.Models.Aggregates;

public class DestinationAttraction : TrackableEntity
{
    public Guid DestinationId { get; init; }
    public Destination Destination { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Category { get; init; } = null!;
    
    private DestinationAttraction() { }
}
