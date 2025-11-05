using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
}
