namespace WebApi.DTOs.Requests.GroupMatch;

public class CreateGroupMatchRequest
{
    public Guid GroupId { get; init; }
    public Guid DestinationId { get; init; }
}
