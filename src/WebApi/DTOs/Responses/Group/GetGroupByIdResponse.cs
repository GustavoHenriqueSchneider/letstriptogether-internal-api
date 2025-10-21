namespace WebApi.DTOs.Responses.Group;

public class GetGroupByIdResponse
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
