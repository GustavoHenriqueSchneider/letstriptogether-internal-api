using System.Text.Json.Serialization;
using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;

public record RegisterCommand : IRequest<RegisterResponse>
{
    public string Password { get; init; } = null!;
    public bool HasAcceptedTermsOfUse { get; init; }
    [JsonIgnore] public string Email { get; init; } = null!;
    [JsonIgnore] public string Name { get; init; } = null!;
}
