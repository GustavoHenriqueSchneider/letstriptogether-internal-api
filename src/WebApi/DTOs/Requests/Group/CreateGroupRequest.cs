namespace WebApi.DTOs.Requests.Group;

public record CreateGroupRequest
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
}
