using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // TODO: verificar regras de senha
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters");

        RuleFor(x => x.HasAcceptedTermsOfUse)
            .Must(x => x == true).WithMessage("Terms of use must be accepted for user registration.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");
    }
}
