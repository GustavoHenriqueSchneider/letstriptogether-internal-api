using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

public class AdminRemoveGroupMemberByIdValidator : AbstractValidator<AdminRemoveGroupMemberByIdCommand>
{
    public AdminRemoveGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}
