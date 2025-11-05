using FluentValidation;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailValidator : AbstractValidator<SendRegisterConfirmationEmailCommand>
{
    public SendRegisterConfirmationEmailValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .SetValidator(new EmailValidator());
    }
}
