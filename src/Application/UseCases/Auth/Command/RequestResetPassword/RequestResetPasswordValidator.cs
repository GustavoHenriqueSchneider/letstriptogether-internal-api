using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Validators;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
{
    public RequestResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator());
    }
}
