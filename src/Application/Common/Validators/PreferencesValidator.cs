using Domain.ValueObjects.TripPreferences;
using FluentValidation;

namespace Application.Common.Validators;

public class CulturePreferencesValidator : AbstractValidator<IEnumerable<string>>
{
    public CulturePreferencesValidator()
    {
        RuleFor(x => x)
            .NotEmpty();
            
        RuleForEach(x => x)
            .NotEmpty()
            .Must(IsValidPreference).WithMessage(y => $"Invalid culture preference '{y}' provided.");
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
            .Must(IsValidPreference).WithMessage(y => $"Invalid entertainment preference '{y}' provided.");
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
            .Must(IsValidPreference).WithMessage(y => $"Invalid place type preference '{y}' provided.");
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