namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Requests.Group;

public record CreateGroupRequest
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
}
