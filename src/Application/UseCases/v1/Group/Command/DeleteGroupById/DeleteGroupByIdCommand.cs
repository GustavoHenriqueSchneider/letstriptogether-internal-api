using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Group.Command.DeleteGroupById;

public class DeleteGroupByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
}
