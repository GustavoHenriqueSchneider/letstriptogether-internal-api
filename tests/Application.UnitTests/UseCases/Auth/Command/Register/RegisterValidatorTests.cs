using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Register;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.Register;

[TestFixture]
public class RegisterValidatorTests
{
    private RegisterValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new RegisterValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = "Test User",
            HasAcceptedTermsOfUse = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithoutAcceptingTerms_ShouldReturnInvalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = "Test User",
            HasAcceptedTermsOfUse = false
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldReturnInvalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "invalid-email",
            Password = "ValidPass123!",
            Name = "Test User",
            HasAcceptedTermsOfUse = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithInvalidPassword_ShouldReturnInvalid()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "test@example.com",
            Password = "short",
            Name = "Test User",
            HasAcceptedTermsOfUse = true
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
