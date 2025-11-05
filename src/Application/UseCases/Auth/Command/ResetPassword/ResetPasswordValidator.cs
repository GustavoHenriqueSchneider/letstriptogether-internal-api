using FluentValidation;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

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
