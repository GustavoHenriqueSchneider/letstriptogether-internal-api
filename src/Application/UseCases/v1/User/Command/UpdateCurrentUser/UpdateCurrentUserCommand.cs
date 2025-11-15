using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.User.Command.UpdateCurrentUser;

public record UpdateCurrentUserCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
}
