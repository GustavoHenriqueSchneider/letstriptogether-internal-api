using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailValidator : AbstractValidator<SendRegisterConfirmationEmailCommand>
{
    public SendRegisterConfirmationEmailValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
