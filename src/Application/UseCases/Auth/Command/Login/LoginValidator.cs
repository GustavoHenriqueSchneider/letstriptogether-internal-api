using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Validators;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
