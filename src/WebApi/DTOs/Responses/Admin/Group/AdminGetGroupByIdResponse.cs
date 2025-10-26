namespace WebApi.DTOs.Responses.Admin.Group;

public class AdminGetGroupByIdResponse
{
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
