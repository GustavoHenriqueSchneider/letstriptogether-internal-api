using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

public class CancelActiveGroupInvitationCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
