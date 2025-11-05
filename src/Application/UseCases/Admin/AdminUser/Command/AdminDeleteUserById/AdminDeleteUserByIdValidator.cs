using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdValidator : AbstractValidator<AdminDeleteUserByIdCommand>
{
    public AdminDeleteUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
