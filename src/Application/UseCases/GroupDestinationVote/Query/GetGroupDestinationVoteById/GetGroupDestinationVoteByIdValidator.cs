using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;

public class GetGroupDestinationVoteByIdValidator : AbstractValidator<GetGroupDestinationVoteByIdQuery>
{
    public GetGroupDestinationVoteByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.DestinationVoteId)
            .NotEmpty().WithMessage("DestinationVoteId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
