using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Invitation.Command.AcceptInvitation;

public record AcceptInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    [JsonIgnore] public Guid UserId { get; init; }
}
