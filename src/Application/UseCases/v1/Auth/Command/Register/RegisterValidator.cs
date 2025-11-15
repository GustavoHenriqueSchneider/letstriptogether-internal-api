using Application.Common.Validators;
using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.Auth.Command.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .SetValidator(new PasswordValidator());

        RuleFor(x => x.HasAcceptedTermsOfUse)
            .Must(x => x).WithMessage("Terms of use must be accepted for user registration.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);
    }
}
