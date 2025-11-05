using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Query.AdminGetUserById;

public class AdminGetUserByIdValidator : AbstractValidator<AdminGetUserByIdQuery>
{
    public AdminGetUserByIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
