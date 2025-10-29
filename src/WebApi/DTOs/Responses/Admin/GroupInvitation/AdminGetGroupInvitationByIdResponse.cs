namespace WebApi.DTOs.Responses.Admin.GroupInvitation;

public class AdminGetGroupInvitationByIdResponse
{
    public Guid GroupId { get; init; }
    public DateTime ExpirationDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

