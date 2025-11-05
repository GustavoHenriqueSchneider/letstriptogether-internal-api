using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;

public class GetAllGroupsValidator : AbstractValidator<GetAllGroupsQuery>
{
    public GetAllGroupsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
