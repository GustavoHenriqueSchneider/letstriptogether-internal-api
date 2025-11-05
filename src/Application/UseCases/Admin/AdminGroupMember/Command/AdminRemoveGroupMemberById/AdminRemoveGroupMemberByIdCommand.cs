using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
}
