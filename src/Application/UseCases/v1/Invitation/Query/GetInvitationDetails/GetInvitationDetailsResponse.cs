namespace Application.UseCases.v1.Invitation.Query.GetInvitationDetails;

public class GetInvitationDetailsResponse
{
    public string CreatedBy { get; init; } = null!;
    public string GroupName { get; init; } = null!;
    public bool IsActive { get; init; }
}
