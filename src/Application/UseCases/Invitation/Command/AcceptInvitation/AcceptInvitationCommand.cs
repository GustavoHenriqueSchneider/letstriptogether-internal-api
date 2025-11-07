using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;

public record AcceptInvitationCommand : IRequest
{
    public string Token { get; init; } = null!;
    [JsonIgnore] public Guid UserId { get; init; }
}
