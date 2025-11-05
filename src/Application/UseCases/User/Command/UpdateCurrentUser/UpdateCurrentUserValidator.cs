using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;

public class UpdateCurrentUserValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        // TODO: nao pode ser string vazia quando informado no body
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");
    }
}
