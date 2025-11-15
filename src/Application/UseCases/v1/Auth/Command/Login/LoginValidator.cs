using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.Auth.Command.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Password)
            .NotEmpty()
            .SetValidator(new PasswordValidator());
    }
}
