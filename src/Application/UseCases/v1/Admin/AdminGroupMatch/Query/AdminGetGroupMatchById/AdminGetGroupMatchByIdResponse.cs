namespace Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdResponse
{
    public Guid DestinationId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
