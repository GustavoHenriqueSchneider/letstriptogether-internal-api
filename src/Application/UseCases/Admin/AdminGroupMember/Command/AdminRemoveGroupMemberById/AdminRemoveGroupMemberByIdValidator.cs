using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdValidator : AbstractValidator<AdminRemoveGroupMemberByIdCommand>
{
    public AdminRemoveGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("MemberId is required");
    }
}
