using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.UpdateGroupById;

public record UpdateGroupByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    [JsonIgnore] public Guid UserId { get; init; }
    public string? Name { get; init; }
    public DateTime? TripExpectedDate { get; init; }
}
