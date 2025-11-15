using Application.UseCases.v1.Auth.Command.Logout;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Auth.Command.Logout;

[TestFixture]
public class LogoutValidatorTests
{
    private LogoutValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new LogoutValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new LogoutCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyUserId_ShouldReturnInvalid()
    {
        // Arrange
        var command = new LogoutCommand
        {
            UserId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
