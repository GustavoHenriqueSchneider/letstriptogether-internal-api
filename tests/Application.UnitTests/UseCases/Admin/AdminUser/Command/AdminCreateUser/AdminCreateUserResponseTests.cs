using Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminCreateUser;

[TestFixture]
public class AdminCreateUserResponseTests
{
    [Test]
    public void AdminCreateUserResponse_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = new AdminCreateUserResponse
        {
            Id = id
        };

        // Assert
        response.Id.Should().Be(id);
    }
}
