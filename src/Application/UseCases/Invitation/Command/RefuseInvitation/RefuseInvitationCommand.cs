using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;

public record RefuseInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    public Guid UserId { get; init; }
}
