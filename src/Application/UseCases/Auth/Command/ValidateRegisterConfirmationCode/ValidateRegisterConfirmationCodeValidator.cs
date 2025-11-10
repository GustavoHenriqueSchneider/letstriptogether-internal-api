using Application.Common.Validators;
using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeValidator : AbstractValidator<ValidateRegisterConfirmationCodeCommand>
{
    public ValidateRegisterConfirmationCodeValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Must(x => int.Parse(x) > 99999).WithMessage("Code must be greater than to 99999")
            .Must(x => int.Parse(x) < 1000000).WithMessage("Code must be less than to 1000000");

        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);
    }
}
