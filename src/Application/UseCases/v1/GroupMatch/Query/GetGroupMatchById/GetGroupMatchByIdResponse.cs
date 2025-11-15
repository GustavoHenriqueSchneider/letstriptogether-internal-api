namespace Application.UseCases.v1.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdResponse
{
    public Guid DestinationId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
