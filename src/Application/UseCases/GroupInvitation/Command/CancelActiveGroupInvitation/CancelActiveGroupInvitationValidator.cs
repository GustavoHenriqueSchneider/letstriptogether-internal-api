using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

public class CancelActiveGroupInvitationValidator : AbstractValidator<CancelActiveGroupInvitationCommand>
{
    public CancelActiveGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
