using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Group.Command.CreateGroup;

public record CreateGroupCommand : IRequest<CreateGroupResponse>
{
    [JsonIgnore] public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
}
