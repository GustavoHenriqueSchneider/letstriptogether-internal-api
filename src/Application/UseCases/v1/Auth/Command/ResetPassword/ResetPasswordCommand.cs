using System.Text.Json.Serialization;
using MediatR;

namespace Application.UseCases.v1.Auth.Command.ResetPassword;

public record ResetPasswordCommand : IRequest
{
    public string Password { get; init; } = null!;
    [JsonIgnore] public Guid UserId { get; init; }
    [JsonIgnore] public string BearerToken { get; init; } = null!;
}
