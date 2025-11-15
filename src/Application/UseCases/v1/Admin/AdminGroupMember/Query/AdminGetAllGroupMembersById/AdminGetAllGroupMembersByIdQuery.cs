using MediatR;

namespace Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;

public class AdminGetAllGroupMembersByIdQuery : IRequest<AdminGetAllGroupMembersByIdResponse>
{
    public Guid GroupId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
