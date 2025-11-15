using FluentValidation;

namespace Application.UseCases.v1.Invitation.Command.RefuseInvitation;

public class RefuseInvitationValidator : AbstractValidator<RefuseInvitationCommand>
{
    public RefuseInvitationValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
