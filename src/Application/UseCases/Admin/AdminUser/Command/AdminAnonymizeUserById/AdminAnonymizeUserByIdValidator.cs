using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminAnonymizeUserById;

public class AdminAnonymizeUserByIdValidator : AbstractValidator<AdminAnonymizeUserByIdCommand>
{
    public AdminAnonymizeUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
