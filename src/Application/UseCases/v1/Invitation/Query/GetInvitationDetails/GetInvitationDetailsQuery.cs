using MediatR;

namespace Application.UseCases.v1.Invitation.Query.GetInvitationDetails;

public class GetInvitationDetailsQuery : IRequest<GetInvitationDetailsResponse>
{
    public string Token { get; init; } = null!;
}
