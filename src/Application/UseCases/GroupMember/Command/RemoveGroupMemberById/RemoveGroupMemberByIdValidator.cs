using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;

public class RemoveGroupMemberByIdValidator : AbstractValidator<RemoveGroupMemberByIdCommand>
{
    public RemoveGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
