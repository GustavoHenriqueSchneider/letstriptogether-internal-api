using FluentValidation;
using LetsTripTogether.InternalApi.Application.Common.Validators;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;

public class SetCurrentUserPreferencesValidator : AbstractValidator<SetCurrentUserPreferencesCommand>
{
    public SetCurrentUserPreferencesValidator()
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
