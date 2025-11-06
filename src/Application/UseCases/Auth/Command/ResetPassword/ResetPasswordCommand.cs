using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public record ResetPasswordCommand : IRequest
{
    public string Password { get; init; } = null!;
    [JsonIgnore] public Guid UserId { get; init; }
    [JsonIgnore] public string BearerToken { get; init; } = null!;
}
