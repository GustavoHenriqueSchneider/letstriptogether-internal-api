using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters");
    }
}
