using Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

[TestFixture]
public class AdminGetAllDestinationsResponseTests
{
    [Test]
    public void AdminGetAllDestinationsResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllDestinationsResponse
        {
            Data = [],
            Hits = 10
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(10);
    }

    [Test]
    public void AdminGetAllDestinationsResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllDestinationsResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
