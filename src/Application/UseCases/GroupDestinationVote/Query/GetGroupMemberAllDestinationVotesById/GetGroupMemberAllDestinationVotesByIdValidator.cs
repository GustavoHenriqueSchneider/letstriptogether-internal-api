using FluentValidation;

namespace Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

public class GetGroupMemberAllDestinationVotesByIdValidator : AbstractValidator<GetGroupMemberAllDestinationVotesByIdQuery>
{
    public GetGroupMemberAllDestinationVotesByIdValidator()
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
