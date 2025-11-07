using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Validators;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.BearerToken)
            .NotEmpty();
    }
}
