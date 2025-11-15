using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdQuery : IRequest<GetGroupMemberByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
