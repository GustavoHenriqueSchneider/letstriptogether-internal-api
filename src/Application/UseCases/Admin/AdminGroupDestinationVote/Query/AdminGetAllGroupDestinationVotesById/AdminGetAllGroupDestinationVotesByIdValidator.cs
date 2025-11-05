using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

public class AdminGetAllGroupDestinationVotesByIdValidator : AbstractValidator<AdminGetAllGroupDestinationVotesByIdQuery>
{
    public AdminGetAllGroupDestinationVotesByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
