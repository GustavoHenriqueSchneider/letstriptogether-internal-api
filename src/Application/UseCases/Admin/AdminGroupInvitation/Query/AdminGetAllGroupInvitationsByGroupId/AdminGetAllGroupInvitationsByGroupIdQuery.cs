using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

public class AdminGetAllGroupInvitationsByGroupIdQuery : IRequest<AdminGetAllGroupInvitationsByGroupIdResponse>
{
    public Guid GroupId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
