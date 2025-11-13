using Application.UseCases.Destination.Query.GetDestinationById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Destination.Query.GetDestinationById;

[TestFixture]
public class GetDestinationByIdResponseTests
{
    [Test]
    public void GetDestinationByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);
        var attractions = new List<DestinationAttractionModel>
        {
            new() { Name = "Attraction 1", Description = "Desc 1", Category = "Cat 1" }
        };

        // Act
        var response = new GetDestinationByIdResponse
        {
            Place = "Test Place",
            Description = "Test Description",
            Attractions = attractions,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.Place.Should().Be("Test Place");
        response.Description.Should().Be("Test Description");
        response.Attractions.Should().HaveCount(1);
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void GetDestinationByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new GetDestinationByIdResponse
        {
            Place = "Test Place",
            Description = "Test Description",
            Attractions = [],
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.Place.Should().Be("Test Place");
        response.Description.Should().Be("Test Description");
        response.Attractions.Should().BeEmpty();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }
}
