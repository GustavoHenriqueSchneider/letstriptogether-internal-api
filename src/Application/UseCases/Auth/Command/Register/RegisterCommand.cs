using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public string Password { get; init; } = null!;
    public bool HasAcceptedTermsOfUse { get; init; }
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
}
