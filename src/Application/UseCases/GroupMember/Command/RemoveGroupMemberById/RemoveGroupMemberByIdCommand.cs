using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;

public class RemoveGroupMemberByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public Guid UserId { get; init; }
}
