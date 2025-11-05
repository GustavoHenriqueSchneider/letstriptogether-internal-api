using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;

public class AcceptInvitationValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
