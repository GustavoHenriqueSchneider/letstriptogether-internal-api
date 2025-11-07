using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

public class CancelActiveGroupInvitationCommand : IRequest
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
