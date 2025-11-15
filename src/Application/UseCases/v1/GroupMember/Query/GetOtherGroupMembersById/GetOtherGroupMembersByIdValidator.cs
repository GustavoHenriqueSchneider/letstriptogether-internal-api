using FluentValidation;

namespace Application.UseCases.v1.GroupMember.Query.GetOtherGroupMembersById;

public class GetOtherGroupMembersByIdValidator : AbstractValidator<GetOtherGroupMembersByIdQuery>
{
    public GetOtherGroupMembersByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
