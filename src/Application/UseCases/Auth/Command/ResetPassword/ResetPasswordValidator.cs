using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.BearerToken)
            .NotEmpty().WithMessage("BearerToken is required");
    }
}
