using FluentValidation;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

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
