using Application.Common.Validators;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.Common.Validators;

[TestFixture]
public class PasswordValidatorTests
{
    private PasswordValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new PasswordValidator();
    }

    [Test]
    public void Validate_WithValidPassword_ShouldReturnValid()
    {
        // Arrange
        const string password = "ValidPass123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyPassword_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithPasswordTooShort_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "Short1!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithPasswordWithoutUppercase_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "lowercase123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("uppercase"));
    }

    [Test]
    public void Validate_WithPasswordWithoutLowercase_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "UPPERCASE123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("lowercase"));
    }

    [Test]
    public void Validate_WithPasswordWithoutNumber_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "NoNumber!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("number"));
    }

    [Test]
    public void Validate_WithPasswordWithoutSpecialCharacter_ShouldReturnInvalid()
    {
        // Arrange
        const string password = "NoSpecial123";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("special character"));
    }
}
