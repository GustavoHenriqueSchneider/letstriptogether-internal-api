using FluentValidation;

namespace Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdValidator : AbstractValidator<AdminGetGroupMemberAllDestinationVotesByIdQuery>
{
    public AdminGetGroupMemberAllDestinationVotesByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
