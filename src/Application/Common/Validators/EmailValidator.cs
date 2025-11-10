using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.Common.Validators;

public class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(UserModel.EmailMaxLength)
            .EmailAddress()
            .WithName("Email");
    }
}