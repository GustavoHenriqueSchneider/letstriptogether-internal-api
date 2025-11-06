using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Query.AdminGetUserById;

[TestFixture]
public class AdminGetUserByIdQueryTests
{
    [Test]
    public void AdminGetUserByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var query = new AdminGetUserByIdQuery
        {
            UserId = userId
        };

        // Assert
        query.UserId.Should().Be(userId);
    }
}
