using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdValidator : AbstractValidator<AdminSetUserPreferencesByUserIdCommand>
{
    public AdminSetUserPreferencesByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Food)
            .NotNull().WithMessage("Food preferences are required.");

        RuleFor(x => x.Culture)
            .NotNull().WithMessage("Culture preferences are required.");

        RuleFor(x => x.Entertainment)
            .NotNull().WithMessage("Entertainment preferences are required.");

        RuleFor(x => x.PlaceTypes)
            .NotNull().WithMessage("Place types are required.");
    }
}
