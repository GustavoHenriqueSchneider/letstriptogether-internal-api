using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdValidator : AbstractValidator<AdminGetGroupMemberByIdQuery>
{
    public AdminGetGroupMemberByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("MemberId is required");
    }
}
