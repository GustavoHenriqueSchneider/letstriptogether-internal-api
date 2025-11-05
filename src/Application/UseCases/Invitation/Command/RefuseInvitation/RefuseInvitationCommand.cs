using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;

public class RefuseInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    public Guid UserId { get; init; }
}
