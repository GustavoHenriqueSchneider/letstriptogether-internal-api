using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdQuery : IRequest<AdminGetGroupInvitationByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid InvitationId { get; init; }
}
