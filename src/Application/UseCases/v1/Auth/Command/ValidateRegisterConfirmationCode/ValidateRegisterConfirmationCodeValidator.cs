using Application.Common.Validators;
using Domain.Security;
using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeValidator : AbstractValidator<ValidateRegisterConfirmationCodeCommand>
{
    public ValidateRegisterConfirmationCodeValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .GreaterThanOrEqualTo(Code.MinValue)
            .LessThanOrEqualTo(Code.MaxValue);

        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);
    }
}
