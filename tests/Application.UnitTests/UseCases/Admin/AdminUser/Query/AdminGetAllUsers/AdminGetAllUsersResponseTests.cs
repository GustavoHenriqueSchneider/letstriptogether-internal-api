using Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;

[TestFixture]
public class AdminGetAllUsersResponseTests
{
    [Test]
    public void AdminGetAllUsersResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllUsersResponse
        {
            Data = [],
            Hits = 10
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(10);
    }

    [Test]
    public void AdminGetAllUsersResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllUsersResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
