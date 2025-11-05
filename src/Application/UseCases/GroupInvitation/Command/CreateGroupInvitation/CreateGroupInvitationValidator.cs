using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;

public class CreateGroupInvitationValidator : AbstractValidator<CreateGroupInvitationCommand>
{
    public CreateGroupInvitationValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
