using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdQuery : IRequest<AdminGetGroupMemberByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
}
