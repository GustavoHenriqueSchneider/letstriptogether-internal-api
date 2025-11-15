using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdValidator : AbstractValidator<AdminDeleteUserByIdCommand>
{
    public AdminDeleteUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
