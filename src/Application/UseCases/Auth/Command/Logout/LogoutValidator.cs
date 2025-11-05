using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
