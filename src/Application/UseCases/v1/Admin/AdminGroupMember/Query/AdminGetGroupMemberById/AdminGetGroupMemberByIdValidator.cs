using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdValidator : AbstractValidator<AdminGetGroupMemberByIdQuery>
{
    public AdminGetGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}
