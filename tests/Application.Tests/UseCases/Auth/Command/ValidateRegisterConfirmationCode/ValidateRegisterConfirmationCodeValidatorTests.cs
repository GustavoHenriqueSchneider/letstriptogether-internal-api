using Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

[TestFixture]
public class ValidateRegisterConfirmationCodeValidatorTests
{
    private ValidateRegisterConfirmationCodeValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new ValidateRegisterConfirmationCodeValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = "test@example.com",
            Name = "Test User",
            Code = "123456"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithInvalidCode_ShouldReturnInvalid()
    {
        // Arrange
        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = "test@example.com",
            Name = "Test User",
            Code = "123"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
