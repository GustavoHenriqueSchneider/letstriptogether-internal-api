using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdValidator : AbstractValidator<AdminGetUserByIdQuery>
{
    public AdminGetUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
