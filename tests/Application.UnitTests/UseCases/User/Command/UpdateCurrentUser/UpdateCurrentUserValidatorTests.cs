using Application.UseCases.User.Command.UpdateCurrentUser;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Command.UpdateCurrentUser;

[TestFixture]
public class UpdateCurrentUserValidatorTests
{
    private UpdateCurrentUserValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateCurrentUserValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new UpdateCurrentUserCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyName_ShouldReturnInvalid()
    {
        // Arrange
        var command = new UpdateCurrentUserCommand
        {
            UserId = Guid.NewGuid(),
            Name = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
