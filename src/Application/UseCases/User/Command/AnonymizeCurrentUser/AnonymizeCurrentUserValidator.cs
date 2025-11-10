using FluentValidation;

namespace Application.UseCases.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserValidator : AbstractValidator<AnonymizeCurrentUserCommand>
{
    public AnonymizeCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
