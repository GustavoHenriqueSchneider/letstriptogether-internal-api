using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdValidator : AbstractValidator<AdminAnonymizeUserByIdCommand>
{
    public AdminAnonymizeUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
