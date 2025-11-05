using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdValidator : AbstractValidator<AdminAnonymizeUserByIdCommand>
{
    public AdminAnonymizeUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
