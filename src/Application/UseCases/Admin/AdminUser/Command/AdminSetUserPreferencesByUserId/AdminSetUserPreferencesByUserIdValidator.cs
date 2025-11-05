using FluentValidation;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdValidator : AbstractValidator<AdminSetUserPreferencesByUserIdCommand>
{
    public AdminSetUserPreferencesByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        
        RuleFor(x => x.LikesCommercial)
            .NotNull();

        RuleFor(x => x.Food)
            .NotEmpty()
            .SetValidator(new FoodPreferencesValidator());

        RuleFor(x => x.Culture)
            .NotEmpty()
            .SetValidator(new CulturePreferencesValidator());

        RuleFor(x => x.Entertainment)
            .NotEmpty()
            .SetValidator(new EntertainmentPreferencesValidator());

        RuleFor(x => x.PlaceTypes)
            .NotEmpty()
            .SetValidator(new PlaceTypePreferencesValidator());
    }
}
