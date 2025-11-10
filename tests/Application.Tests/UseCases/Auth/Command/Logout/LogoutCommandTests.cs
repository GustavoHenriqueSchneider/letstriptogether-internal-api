using Application.UseCases.Auth.Command.Logout;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.Logout;

[TestFixture]
public class LogoutCommandTests
{
    [Test]
    public void LogoutCommand_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new LogoutCommand
        {
            UserId = userId
        };

        // Assert
        command.UserId.Should().Be(userId);
    }
}
