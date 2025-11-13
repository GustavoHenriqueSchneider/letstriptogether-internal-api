using Application.UseCases.User.Command.SetCurrentUserPreferences;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Command.SetCurrentUserPreferences;

[TestFixture]
public class SetCurrentUserPreferencesValidatorTests
{
    private SetCurrentUserPreferencesValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new SetCurrentUserPreferencesValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = Guid.NewGuid(),
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            Entertainment = new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Beach) }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyUserId_ShouldReturnInvalid()
    {
        // Arrange
        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = Guid.Empty,
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = new List<string>(),
            Entertainment = new List<string>(),
            PlaceTypes = new List<string>()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
