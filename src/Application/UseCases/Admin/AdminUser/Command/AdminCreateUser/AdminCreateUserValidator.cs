using FluentValidation;
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
            .NotEmpty()
            .SetValidator(new EmailValidator());
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .SetValidator(new PasswordValidator());
    }
}
