using Application.UseCases.Auth.Command.RequestResetPassword;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.RequestResetPassword;

[TestFixture]
public class RequestResetPasswordValidatorTests
{
    private RequestResetPasswordValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new RequestResetPasswordValidator();
    }

    [Test]
    public void Validate_WithValidEmail_ShouldReturnValid()
    {
        // Arrange
        var command = new RequestResetPasswordCommand
        {
            Email = "test@example.com"
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
        var command = new RequestResetPasswordCommand
        {
            Email = "invalid-email"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
