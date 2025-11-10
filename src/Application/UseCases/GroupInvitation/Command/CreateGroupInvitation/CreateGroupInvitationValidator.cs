using FluentValidation;

namespace Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationValidator : AbstractValidator<CreateGroupInvitationCommand>
{
    public CreateGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
