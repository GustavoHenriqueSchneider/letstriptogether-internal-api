using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

[TestFixture]
public class AdminAnonymizeUserByIdCommandTests
{
    [Test]
    public void AdminAnonymizeUserByIdCommand_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new AdminAnonymizeUserByIdCommand
        {
            UserId = userId
        };

        // Assert
        command.UserId.Should().Be(userId);
    }
}
