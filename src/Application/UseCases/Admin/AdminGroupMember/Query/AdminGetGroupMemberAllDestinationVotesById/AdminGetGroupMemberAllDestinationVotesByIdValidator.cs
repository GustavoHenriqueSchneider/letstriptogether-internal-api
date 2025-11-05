using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdValidator : AbstractValidator<AdminGetGroupMemberAllDestinationVotesByIdQuery>
{
    public AdminGetGroupMemberAllDestinationVotesByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MemberId)
            .NotEmpty().WithMessage("MemberId is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
