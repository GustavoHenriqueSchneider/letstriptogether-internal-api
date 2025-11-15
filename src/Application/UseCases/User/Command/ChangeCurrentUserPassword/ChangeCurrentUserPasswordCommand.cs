using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.User.Command.ChangeCurrentUserPassword;

public record ChangeCurrentUserPasswordCommand : IRequest
{
    [JsonIgnore] public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
}
