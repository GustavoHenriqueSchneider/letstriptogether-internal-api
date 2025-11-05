using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationCommand : IRequest<CreateGroupInvitationResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
