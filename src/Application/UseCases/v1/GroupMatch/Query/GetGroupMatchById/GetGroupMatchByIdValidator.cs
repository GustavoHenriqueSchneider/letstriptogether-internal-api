using FluentValidation;

namespace Application.UseCases.v1.GroupMatch.Query.GetGroupMatchById;

public class GetGroupMatchByIdValidator : AbstractValidator<GetGroupMatchByIdQuery>
{
    public GetGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MatchId)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
