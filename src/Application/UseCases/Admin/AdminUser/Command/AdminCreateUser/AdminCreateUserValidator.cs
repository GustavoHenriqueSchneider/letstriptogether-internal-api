using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminCreateUser;

public class AdminCreateUserValidator : AbstractValidator<AdminCreateUserCommand>
{
    public AdminCreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters")
            .EmailAddress().WithMessage("Invalid email format");

        // TODO: verificar regras de senha
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters");
    }
}
