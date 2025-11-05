using FluentValidation;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using UserModel = LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User;

namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;

public class FoodPreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public FoodPreferencesValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(x =>
            {
                x.ChildRules(preference =>
                {
                    preference.RuleFor(y => y)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.")
                        .When(y => !string.IsNullOrEmpty(y));
                });
            });
    }
    
    private static bool IsValidPreference(string value)
    {
        try
        {
            _ = new TripPreference.Food(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class CulturePreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public CulturePreferencesValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(x =>
            {
                x.ChildRules(preference =>
                {
                    preference.RuleFor(y => y)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.")
                        .When(y => !string.IsNullOrEmpty(y));
                });
            });
    }
    
    private static bool IsValidPreference(string value)
    {
        try
        {
            _ = new TripPreference.Culture(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class EntertainmentPreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public EntertainmentPreferencesValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(x =>
            {
                x.ChildRules(preference =>
                {
                    preference.RuleFor(y => y)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.")
                        .When(y => !string.IsNullOrEmpty(y));
                });
            });
    }
    
    private static bool IsValidPreference(string value)
    {
        try
        {
            _ = new TripPreference.Entertainment(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class PlaceTypePreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public PlaceTypePreferencesValidator()
    {
        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(x =>
            {
                x.ChildRules(preference =>
                {
                    preference.RuleFor(y => y)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.")
                        .When(y => !string.IsNullOrEmpty(y));
                });
            });
    }
    
    private static bool IsValidPreference(string value)
    {
        try
        {
            _ = new TripPreference.PlaceType(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}