using FluentValidation;

namespace Application.UseCases.v1.GroupMatch.Query.GetAllGroupMatchesById;

public class GetAllGroupMatchesByIdValidator : AbstractValidator<GetAllGroupMatchesByIdQuery>
{
    public GetAllGroupMatchesByIdValidator()
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
