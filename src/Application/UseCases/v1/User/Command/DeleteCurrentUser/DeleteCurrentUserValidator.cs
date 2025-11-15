using FluentValidation;

namespace Application.UseCases.v1.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserValidator : AbstractValidator<DeleteCurrentUserCommand>
{
    public DeleteCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
