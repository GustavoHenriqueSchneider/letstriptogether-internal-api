using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.v1.Auth.Command.RequestResetPassword;

public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
{
    public RequestResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());
    }
}
