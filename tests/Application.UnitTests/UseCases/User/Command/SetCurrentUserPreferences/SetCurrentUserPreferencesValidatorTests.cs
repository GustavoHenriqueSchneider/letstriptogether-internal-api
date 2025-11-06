using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;
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
            LikesCommercial = true,
            Food = new List<string> { "restaurant" },
            Culture = new List<string> { "museum" },
            Entertainment = new List<string> { "attraction" },
            PlaceTypes = new List<string> { "beach" }
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
            LikesCommercial = true,
            Food = new List<string>(),
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
