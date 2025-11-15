using FluentValidation;

namespace Application.UseCases.v1.GroupInvitation.Command.CancelActiveGroupInvitation;

public class CancelActiveGroupInvitationValidator : AbstractValidator<CancelActiveGroupInvitationCommand>
{
    public CancelActiveGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
