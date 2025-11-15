using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
}
