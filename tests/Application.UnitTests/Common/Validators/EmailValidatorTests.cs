using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Validators;
using NUnit.Framework;

namespace Application.UnitTests.Common.Validators;

[TestFixture]
public class EmailValidatorTests
{
    private EmailValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new EmailValidator();
    }

    [Test]
    public void Validate_WithValidEmail_ShouldReturnValid()
    {
        // Arrange
        const string email = "test@example.com";

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyEmail_ShouldReturnInvalid()
    {
        // Arrange
        const string email = "";

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public void Validate_WithInvalidEmailFormat_ShouldReturnInvalid()
    {
        // Arrange
        const string email = "invalid-email";

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithEmailExceedingMaxLength_ShouldReturnInvalid()
    {
        // Arrange
        var email = new string('a', 250) + "@example.com"; // Exceeds 254 characters

        // Act
        var result = _validator.Validate(email);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithNullEmail_ShouldReturnInvalid()
    {
        // Arrange
        string? email = null;

        // Act
        var result = _validator.Validate(email!);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
