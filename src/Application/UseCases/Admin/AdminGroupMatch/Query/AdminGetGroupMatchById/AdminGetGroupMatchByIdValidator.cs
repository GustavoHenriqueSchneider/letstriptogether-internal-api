using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdValidator : AbstractValidator<AdminGetGroupMatchByIdQuery>
{
    public AdminGetGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("GroupId is required");

        RuleFor(x => x.MatchId)
            .NotEmpty().WithMessage("MatchId is required");
    }
}
