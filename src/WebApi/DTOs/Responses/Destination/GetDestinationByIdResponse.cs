namespace WebApi.DTOs.Responses.Destination;

public class GetDestinationByIdResponse
{
    public string Address { get; init; } = null!;
    public List<string> Categories { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
