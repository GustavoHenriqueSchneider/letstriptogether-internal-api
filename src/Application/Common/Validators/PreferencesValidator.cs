using FluentValidation;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;

namespace LetsTripTogether.InternalApi.Application.Common.Validators;

public class FoodPreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public FoodPreferencesValidator()
    {
        RuleFor(x => x)
            .NotEmpty();
            
        RuleForEach(x => x)
            .NotEmpty()
            .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.");
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
            .NotEmpty();
            
        RuleForEach(x => x)
            .NotEmpty()
            .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.");
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
            .NotEmpty();
            
        RuleForEach(x => x)
            .NotEmpty()
            .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.");
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
            .NotEmpty();
            
        RuleForEach(x => x)
            .NotEmpty()
            .Must(IsValidPreference).WithMessage(y => $"Invalid food preference '{y}' provided.");
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