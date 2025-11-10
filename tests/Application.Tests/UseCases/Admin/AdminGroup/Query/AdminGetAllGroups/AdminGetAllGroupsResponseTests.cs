using Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

[TestFixture]
public class AdminGetAllGroupsResponseTests
{
    [Test]
    public void AdminGetAllGroupsResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllGroupsResponse
        {
            Data = [],
            Hits = 5
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(5);
    }

    [Test]
    public void AdminGetAllGroupsResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllGroupsResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
