using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminDeleteUserById;

public class AdminDeleteUserByIdValidator : AbstractValidator<AdminDeleteUserByIdCommand>
{
    public AdminDeleteUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
