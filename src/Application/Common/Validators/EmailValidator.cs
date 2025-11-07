using FluentValidation;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.Common.Validators;

public class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(UserModel.EmailMaxLength)
            .EmailAddress()
            .WithName("Email");
    }
}