using FluentValidation;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
{
    public RequestResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());
    }
}
