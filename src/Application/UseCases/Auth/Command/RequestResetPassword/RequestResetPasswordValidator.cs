using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordValidator : AbstractValidator<RequestResetPasswordCommand>
{
    public RequestResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
