using FluentValidation;

namespace Application.UseCases.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdValidator : AbstractValidator<GetGroupMemberByIdQuery>
{
    public GetGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
