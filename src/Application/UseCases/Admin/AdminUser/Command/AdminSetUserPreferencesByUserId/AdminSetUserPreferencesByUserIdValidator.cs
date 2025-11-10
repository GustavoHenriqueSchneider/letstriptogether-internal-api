using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdValidator : AbstractValidator<AdminSetUserPreferencesByUserIdCommand>
{
    public AdminSetUserPreferencesByUserIdValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
        
        RuleFor(x => x.LikesCommercial)
            .NotNull();

        RuleFor(x => x.Food)
            .SetValidator(new FoodPreferencesValidator());

        RuleFor(x => x.Culture)
            .SetValidator(new CulturePreferencesValidator());

        RuleFor(x => x.Entertainment)
            .SetValidator(new EntertainmentPreferencesValidator());

        RuleFor(x => x.PlaceTypes)
            .SetValidator(new PlaceTypePreferencesValidator());
    }
}
