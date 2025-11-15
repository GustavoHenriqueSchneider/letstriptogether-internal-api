using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .SetValidator(new PasswordValidator());

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BearerToken)
            .NotEmpty();
    }
}
