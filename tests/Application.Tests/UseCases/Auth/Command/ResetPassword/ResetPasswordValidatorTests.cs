using Application.UseCases.Auth.Command.ResetPassword;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.ResetPassword;

[TestFixture]
public class ResetPasswordValidatorTests
{
    private ResetPasswordValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new ResetPasswordValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            UserId = Guid.NewGuid(),
            Password = "ValidPass123!",
            BearerToken = "valid-token"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyPassword_ShouldReturnInvalid()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            UserId = Guid.NewGuid(),
            Password = "",
            BearerToken = "valid-token"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
