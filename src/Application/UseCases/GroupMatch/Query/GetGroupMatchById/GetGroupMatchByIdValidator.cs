using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdValidator : AbstractValidator<GetGroupMatchByIdQuery>
{
    public GetGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("MatchId is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
