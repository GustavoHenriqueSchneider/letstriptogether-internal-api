using MediatR;

namespace Application.UseCases.Invitation.Query.GetInvitationDetails;

public class GetInvitationDetailsQuery : IRequest<GetInvitationDetailsResponse>
{
    public string Token { get; init; } = null!;
}
