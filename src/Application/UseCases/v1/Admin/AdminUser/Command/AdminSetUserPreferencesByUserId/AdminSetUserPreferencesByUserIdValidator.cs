using Application.Common.Validators;
using FluentValidation;

namespace Application.UseCases.v1.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

public class AdminSetUserPreferencesByUserIdValidator : AbstractValidator<AdminSetUserPreferencesByUserIdCommand>
{
    public AdminSetUserPreferencesByUserIdValidator()
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
