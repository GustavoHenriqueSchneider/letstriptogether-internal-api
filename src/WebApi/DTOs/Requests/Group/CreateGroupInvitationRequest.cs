namespace WebApi.DTOs.Requests.Group;

public record CreateGroupInvitationRequest
{
    public DateTime ExpirationDate { get; init; }
}

