using Application.UseCases.Auth.Command.Login;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.Login;

[TestFixture]
public class LoginValidatorTests
{
    private LoginValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new LoginValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldReturnInvalid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "invalid-email",
            Password = "ValidPass123!"
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
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "short"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
