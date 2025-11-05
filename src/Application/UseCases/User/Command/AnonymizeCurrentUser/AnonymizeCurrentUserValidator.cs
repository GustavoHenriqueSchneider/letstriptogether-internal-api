using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserValidator : AbstractValidator<AnonymizeCurrentUserCommand>
{
    public AnonymizeCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
