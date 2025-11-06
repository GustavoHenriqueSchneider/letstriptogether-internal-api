using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Validators;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using NUnit.Framework;

namespace Application.UnitTests.Common.Validators;

[TestFixture]
public class FoodPreferencesValidatorTests
{
    private FoodPreferencesValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new FoodPreferencesValidator();
    }

    [Test]
    public void Validate_WithValidPreferences_ShouldReturnValid()
    {
        // Arrange
        var preferences = new List<string> { new TripPreference(TripPreference.Food.Restaurant) };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = Array.Empty<string>();

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidPreference_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "InvalidPreference" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}

[TestFixture]
public class CulturePreferencesValidatorTests
{
    private CulturePreferencesValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CulturePreferencesValidator();
    }

    [Test]
    public void Validate_WithValidPreferences_ShouldReturnValid()
    {
        // Arrange
        var preferences = new[] { TripPreference.Culture.Museum, "monument" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = Array.Empty<string>();

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidPreference_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "invalid.culture.preference" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithEmptyStringInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithNullInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new string[] { null! };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}

[TestFixture]
public class EntertainmentPreferencesValidatorTests
{
    private EntertainmentPreferencesValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new EntertainmentPreferencesValidator();
    }

    [Test]
    public void Validate_WithValidPreferences_ShouldReturnValid()
    {
        // Arrange
        var preferences = new[] { new TripPreference(TripPreference.Entertainment.Attraction), "park" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = Array.Empty<string>();

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidPreference_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "invalid.entertainment.preference" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithEmptyStringInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithNullInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new string[] { null! };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}

[TestFixture]
public class PlaceTypePreferencesValidatorTests
{
    private PlaceTypePreferencesValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new PlaceTypePreferencesValidator();
    }

    [Test]
    public void Validate_WithValidPreferences_ShouldReturnValid()
    {
        // Arrange
        var preferences = new[] { new TripPreference(TripPreference.PlaceType.Beach), "mountain" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = Array.Empty<string>();

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidPreference_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "invalid.placetype.preference" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithEmptyStringInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new[] { "" };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithNullInCollection_ShouldReturnInvalid()
    {
        // Arrange
        var preferences = new string[] { null! };

        // Act
        var result = _validator.Validate(preferences);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
