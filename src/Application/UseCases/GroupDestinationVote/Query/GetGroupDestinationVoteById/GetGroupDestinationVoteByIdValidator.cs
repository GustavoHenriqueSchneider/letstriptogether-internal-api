using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

public class GetGroupDestinationVoteByIdValidator : AbstractValidator<GetGroupDestinationVoteByIdQuery>
{
    public GetGroupDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
