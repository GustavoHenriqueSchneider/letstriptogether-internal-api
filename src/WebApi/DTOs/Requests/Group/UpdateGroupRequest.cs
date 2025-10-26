namespace WebApi.DTOs.Requests.Group;

public record UpdateGroupRequest
{
    public string? Name { get; init; } = null!;
    public DateTime? TripExpectedDate { get; init; }
}
