using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
