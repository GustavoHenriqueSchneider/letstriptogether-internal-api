using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;

public record CreateGroupCommand : IRequest<CreateGroupResponse>
{
    [JsonIgnore] public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime TripExpectedDate { get; init; }
}
