using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Validators;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

public class AdminCreateUserValidator : AbstractValidator<AdminCreateUserCommand>
{
    public AdminCreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(UserModel.NameMaxLength);

        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator());
        
        RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
