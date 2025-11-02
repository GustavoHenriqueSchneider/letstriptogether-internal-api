namespace WebApi.DTOs.Responses.Destination;

public class GetDestinationByIdResponse
{
    public string Place { get; init; } = null!;
    public string Description { get; init; } = null!;
    public IEnumerable<DestinationAttractionModel> Attractions { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class DestinationAttractionModel
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Category { get; init; } = null!;
}
