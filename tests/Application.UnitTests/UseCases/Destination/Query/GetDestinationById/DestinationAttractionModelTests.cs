using Application.UseCases.v1.Destination.Query.GetDestinationById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Destination.Query.GetDestinationById;

[TestFixture]
public class DestinationAttractionModelTests
{
    [Test]
    public void DestinationAttractionModel_ShouldSetProperties()
    {
        // Arrange & Act
        var model = new DestinationAttractionModel
        {
            Name = "Test Attraction",
            Description = "Test Description",
            Category = "Test Category"
        };

        // Assert
        model.Name.Should().Be("Test Attraction");
        model.Description.Should().Be("Test Description");
        model.Category.Should().Be("Test Category");
    }
}
