using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;

public class GetOtherGroupMembersByIdQuery : IRequest<GetOtherGroupMembersByIdResponse>
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
