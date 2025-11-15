using FluentValidation;

namespace Application.UseCases.v1.Auth.Command.Logout;

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
