using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;

public class RemoveGroupMemberByIdValidator : AbstractValidator<RemoveGroupMemberByIdCommand>
{
    public RemoveGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("MemberId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
