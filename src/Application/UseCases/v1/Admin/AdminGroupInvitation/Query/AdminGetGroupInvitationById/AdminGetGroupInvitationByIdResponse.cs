namespace Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdResponse
{
    public string Status { get; init; } = null!;
    public DateTime ExpirationDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
