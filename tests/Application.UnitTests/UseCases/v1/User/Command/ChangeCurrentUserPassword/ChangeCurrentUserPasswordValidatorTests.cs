using Application.UseCases.v1.User.Command.ChangeCurrentUserPassword;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.User.Command.ChangeCurrentUserPassword;

[TestFixture]
public class ChangeCurrentUserPasswordValidatorTests
{
    private ChangeCurrentUserPasswordValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new ChangeCurrentUserPasswordValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "ValidPassword123!",
            NewPassword = "NewValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.Empty,
            CurrentPassword = "ValidPassword123!",
            NewPassword = "NewValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Test]
    public void Validate_WithEmptyCurrentPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = string.Empty,
            NewPassword = "NewValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("CurrentPassword.Password");
    }

    [Test]
    public void Validate_WithEmptyNewPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "ValidPassword123!",
            NewPassword = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("NewPassword.Password");
    }

    [Test]
    public void Validate_WithInvalidCurrentPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "short",
            NewPassword = "NewValidPassword123!"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("CurrentPassword.Password");
    }

    [Test]
    public void Validate_WithInvalidNewPassword_ShouldHaveValidationError()
    {
        // Arrange
        var command = new ChangeCurrentUserPasswordCommand
        {
            UserId = Guid.NewGuid(),
            CurrentPassword = "ValidPassword123!",
            NewPassword = "short"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("NewPassword.Password");
    }
}
