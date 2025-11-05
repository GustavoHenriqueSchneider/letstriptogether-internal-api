using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public record ValidateRegisterConfirmationCodeCommand : IRequest<ValidateRegisterConfirmationCodeResponse>
{
    public string Code { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
}
