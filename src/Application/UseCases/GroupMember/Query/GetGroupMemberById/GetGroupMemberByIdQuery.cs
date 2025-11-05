using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdQuery : IRequest<GetGroupMemberByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public Guid UserId { get; init; }
}
