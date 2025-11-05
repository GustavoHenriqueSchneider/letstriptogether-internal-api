using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminUpdateUserById;

public class AdminUpdateUserByIdValidator : AbstractValidator<AdminUpdateUserByIdCommand>
{
    public AdminUpdateUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        // TODO: nao pode ser string vazia quando informado no body
        RuleFor(x => x.Name)
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters")
            .When(x => x.Name is not null);
    }
}
