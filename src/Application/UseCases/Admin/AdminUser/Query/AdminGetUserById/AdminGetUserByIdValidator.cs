using FluentValidation;

namespace Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdValidator : AbstractValidator<AdminGetUserByIdQuery>
{
    public AdminGetUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
