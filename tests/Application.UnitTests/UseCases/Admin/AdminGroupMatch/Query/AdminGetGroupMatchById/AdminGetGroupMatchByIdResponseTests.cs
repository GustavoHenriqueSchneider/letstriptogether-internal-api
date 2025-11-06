using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

[TestFixture]
public class AdminGetGroupMatchByIdResponseTests
{
    [Test]
    public void AdminGetGroupMatchByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var destinationId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var response = new AdminGetGroupMatchByIdResponse
        {
            DestinationId = destinationId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.DestinationId.Should().Be(destinationId);
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetGroupMatchByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var destinationId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetGroupMatchByIdResponse
        {
            DestinationId = destinationId,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.DestinationId.Should().Be(destinationId);
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }
}
