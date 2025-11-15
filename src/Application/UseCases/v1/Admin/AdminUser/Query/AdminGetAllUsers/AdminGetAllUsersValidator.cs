using FluentValidation;

namespace Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;

public class AdminGetAllUsersValidator : AbstractValidator<AdminGetAllUsersQuery>
{
    public AdminGetAllUsersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}
