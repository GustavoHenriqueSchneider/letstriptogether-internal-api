using FluentValidation;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.Common.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(UserModel.PasswordMinLength)
            .MaximumLength(UserModel.PasswordMaxLength)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
            .WithName("Password");
    }
}