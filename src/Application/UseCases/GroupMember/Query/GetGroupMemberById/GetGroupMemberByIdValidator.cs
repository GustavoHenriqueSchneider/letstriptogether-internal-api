using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Query.GetGroupMemberById;

public class GetGroupMemberByIdValidator : AbstractValidator<GetGroupMemberByIdQuery>
{
    public GetGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("MemberId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
