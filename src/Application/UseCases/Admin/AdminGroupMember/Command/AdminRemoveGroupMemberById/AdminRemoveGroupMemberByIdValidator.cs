using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

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
