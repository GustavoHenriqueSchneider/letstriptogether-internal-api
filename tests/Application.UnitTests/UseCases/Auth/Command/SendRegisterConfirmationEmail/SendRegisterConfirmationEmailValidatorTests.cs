using Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.SendRegisterConfirmationEmail;

[TestFixture]
public class SendRegisterConfirmationEmailValidatorTests
{
    private SendRegisterConfirmationEmailValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new SendRegisterConfirmationEmailValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new SendRegisterConfirmationEmailCommand
        {
            Email = "test@example.com",
            Name = "Test User"
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
        var command = new SendRegisterConfirmationEmailCommand
        {
            Email = "invalid-email",
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
