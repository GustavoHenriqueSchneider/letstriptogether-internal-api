using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

public class AdminGetGroupMatchByIdValidator : AbstractValidator<AdminGetGroupMatchByIdQuery>
{
    public AdminGetGroupMatchByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.MatchId)
            .NotEmpty();
    }
}
