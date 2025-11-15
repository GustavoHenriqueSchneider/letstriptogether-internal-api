using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.GroupInvitation.Query.GetActiveGroupInvitation;

public class GetActiveGroupInvitationQuery : IRequest<GetActiveGroupInvitationResponse>
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
