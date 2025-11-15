using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.Invitation.Command.RefuseInvitation;

public record RefuseInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    [JsonIgnore] public Guid UserId { get; init; }
}
