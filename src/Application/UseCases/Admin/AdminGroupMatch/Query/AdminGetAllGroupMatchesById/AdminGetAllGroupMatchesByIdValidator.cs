using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdValidator : AbstractValidator<AdminGetAllGroupMatchesByIdQuery>
{
    public AdminGetAllGroupMatchesByIdValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
