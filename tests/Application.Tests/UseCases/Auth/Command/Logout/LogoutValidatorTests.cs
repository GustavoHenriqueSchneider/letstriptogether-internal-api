using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.Logout;

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
