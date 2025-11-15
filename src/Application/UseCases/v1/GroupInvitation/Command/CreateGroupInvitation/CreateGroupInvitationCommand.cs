using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationCommand : IRequest<CreateGroupInvitationResponse>
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
