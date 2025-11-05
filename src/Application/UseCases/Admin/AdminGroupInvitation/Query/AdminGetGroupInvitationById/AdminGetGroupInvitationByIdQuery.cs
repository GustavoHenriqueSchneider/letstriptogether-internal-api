using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdQuery : IRequest<AdminGetGroupInvitationByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid InvitationId { get; init; }
}
