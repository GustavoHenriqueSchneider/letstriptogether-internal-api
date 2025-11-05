using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;

public class AdminGetAllGroupMembersByIdValidator : AbstractValidator<AdminGetAllGroupMembersByIdQuery>
{
    public AdminGetAllGroupMembersByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
