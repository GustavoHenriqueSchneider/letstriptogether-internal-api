using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;

public class AcceptInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    public Guid UserId { get; init; }
}
