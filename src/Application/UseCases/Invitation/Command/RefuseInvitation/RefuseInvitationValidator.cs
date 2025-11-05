using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;

public class RefuseInvitationValidator : AbstractValidator<RefuseInvitationCommand>
{
    public RefuseInvitationValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
