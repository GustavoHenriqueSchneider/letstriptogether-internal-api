namespace WebApi.DTOs.Requests.GroupDestinationVote;

public class VoteAtDestinationForGroupIdRequest
{
    public Guid DestinationId { get; set; }
    public bool IsApproved { get; set; }
}