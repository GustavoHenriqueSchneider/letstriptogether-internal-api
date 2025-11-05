using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserValidator : AbstractValidator<DeleteCurrentUserCommand>
{
    public DeleteCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
