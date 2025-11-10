using FluentValidation;

namespace Application.UseCases.Invitation.Command.AcceptInvitation;

public class AcceptInvitationValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
