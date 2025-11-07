using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

[TestFixture]
public class AdminSetUserPreferencesByUserIdValidatorTests
{
    private AdminSetUserPreferencesByUserIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminSetUserPreferencesByUserIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = Guid.NewGuid(), 
            LikesCommercial = true, 
            Food = new List<string> { new TripPreference(TripPreference.Food.Restaurant) }, 
            Culture = new List<string> { TripPreference.Culture.Architecture }, 
            Entertainment = new List<string> { TripPreference.Entertainment.Adventure }, 
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Beach) }
        };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
