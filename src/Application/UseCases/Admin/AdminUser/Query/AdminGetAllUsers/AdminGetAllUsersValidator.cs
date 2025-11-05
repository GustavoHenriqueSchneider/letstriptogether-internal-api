using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Query.AdminGetAllUsers;

public class AdminGetAllUsersValidator : AbstractValidator<AdminGetAllUsersQuery>
{
    public AdminGetAllUsersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}
