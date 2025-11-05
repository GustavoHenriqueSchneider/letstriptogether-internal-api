using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

public class AdminGetGroupInvitationByIdQuery : IRequest<AdminGetGroupInvitationByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid InvitationId { get; init; }
}
