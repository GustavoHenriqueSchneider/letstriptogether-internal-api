namespace WebApi.DTOs.Responses.Admin.GroupMatch;

public class GetGroupMatchByIdResponse
{
    public Guid DestinationId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
