using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Group.Query.GetGroupById;

public class GetGroupByIdQuery : IRequest<GetGroupByIdResponse>
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
