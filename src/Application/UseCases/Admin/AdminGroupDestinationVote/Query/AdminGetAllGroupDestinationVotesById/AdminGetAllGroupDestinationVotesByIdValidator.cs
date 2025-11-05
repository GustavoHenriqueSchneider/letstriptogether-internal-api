using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

public class AdminGetAllGroupDestinationVotesByIdValidator : AbstractValidator<AdminGetAllGroupDestinationVotesByIdQuery>
{
    public AdminGetAllGroupDestinationVotesByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
