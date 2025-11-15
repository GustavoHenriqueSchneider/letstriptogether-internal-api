using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.v1.User.Command.ChangeCurrentUserPassword;

public class ChangeCurrentUserPasswordValidator : AbstractValidator<ChangeCurrentUserPasswordCommand>
{
    public ChangeCurrentUserPasswordValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .SetValidator(new PasswordValidator());

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .SetValidator(new PasswordValidator());
    }
}
