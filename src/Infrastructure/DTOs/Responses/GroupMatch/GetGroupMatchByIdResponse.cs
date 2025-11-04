namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMatch;

public class GetGroupMatchByIdResponse
{
    public Guid DestinationId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
