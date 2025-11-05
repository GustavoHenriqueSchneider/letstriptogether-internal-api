using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdQuery : IRequest<AdminGetGroupMemberByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
}
