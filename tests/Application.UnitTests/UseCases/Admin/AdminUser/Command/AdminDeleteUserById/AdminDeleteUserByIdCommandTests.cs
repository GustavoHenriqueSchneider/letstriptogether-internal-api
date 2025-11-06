using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

[TestFixture]
public class AdminDeleteUserByIdCommandTests
{
    [Test]
    public void AdminDeleteUserByIdCommand_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new AdminDeleteUserByIdCommand
        {
            UserId = userId
        };

        // Assert
        command.UserId.Should().Be(userId);
    }
}
