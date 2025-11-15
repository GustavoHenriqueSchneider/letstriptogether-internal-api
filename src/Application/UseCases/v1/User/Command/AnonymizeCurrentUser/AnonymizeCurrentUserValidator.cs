using FluentValidation;

namespace Application.UseCases.v1.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserValidator : AbstractValidator<AnonymizeCurrentUserCommand>
{
    public AnonymizeCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
