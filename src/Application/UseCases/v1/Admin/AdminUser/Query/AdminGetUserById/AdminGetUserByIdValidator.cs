using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdValidator : AbstractValidator<AdminGetUserByIdQuery>
{
    public AdminGetUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
