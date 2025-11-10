using Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;

[TestFixture]
public class AdminUpdateUserByIdCommandTests
{
    [Test]
    public void AdminUpdateUserByIdCommand_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = "Test Name";

        // Act
        var command = new AdminUpdateUserByIdCommand
        {
            UserId = userId,
            Name = name
        };

        // Assert
        command.UserId.Should().Be(userId);
        command.Name.Should().Be(name);
    }

    [Test]
    public void AdminUpdateUserByIdCommand_WithNullName_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new AdminUpdateUserByIdCommand
        {
            UserId = userId,
            Name = null
        };

        // Assert
        command.UserId.Should().Be(userId);
        command.Name.Should().BeNull();
    }
}
