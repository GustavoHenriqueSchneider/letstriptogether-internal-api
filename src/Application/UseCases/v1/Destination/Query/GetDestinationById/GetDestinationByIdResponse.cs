namespace Application.UseCases.v1.Destination.Query.GetDestinationById;

public class DestinationAttractionModel
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Category { get; init; } = null!;
}

public class GetDestinationByIdResponse
{
    public string Place { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Image { get; init; } = string.Empty;
    public IEnumerable<DestinationAttractionModel> Attractions { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
