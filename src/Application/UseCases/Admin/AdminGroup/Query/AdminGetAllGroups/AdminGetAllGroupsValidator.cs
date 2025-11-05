using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsValidator : AbstractValidator<AdminGetAllGroupsQuery>
{
    public AdminGetAllGroupsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
