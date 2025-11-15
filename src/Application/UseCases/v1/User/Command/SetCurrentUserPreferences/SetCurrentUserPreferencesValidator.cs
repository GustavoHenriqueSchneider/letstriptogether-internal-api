using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.v1.User.Command.SetCurrentUserPreferences;

public class SetCurrentUserPreferencesValidator : AbstractValidator<SetCurrentUserPreferencesCommand>
{
    public SetCurrentUserPreferencesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.LikesShopping)
            .NotNull();

        RuleFor(x => x.LikesGastronomy)
            .NotNull();

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
