using FluentValidation;
using UserModel = Domain.Aggregates.UserAggregate.Entities.User;

namespace Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

public class AdminUpdateUserByIdValidator : AbstractValidator<AdminUpdateUserByIdCommand>
{
    public AdminUpdateUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        
        RuleFor(x => x.Name)
            .MaximumLength(UserModel.NameMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Name));
    }
}
