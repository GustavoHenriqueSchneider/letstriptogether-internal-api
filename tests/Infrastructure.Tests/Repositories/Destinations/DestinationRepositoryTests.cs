using Domain.Aggregates.DestinationAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Destinations;
using Infrastructure.Tests.Common;
using NUnit.Framework;

namespace Infrastructure.Tests.Repositories.Destinations;

[TestFixture]
public class DestinationRepositoryTests : TestBase
{
    private DestinationRepository _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        _repository = new DestinationRepository(DbContext);
    }

    [Test]
    public async Task GetAllAsync_WithDestinations_ShouldReturnPaginatedResultsWithAttractions()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var destination = new Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await _repository.AddAsync(destination, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var (data, hits) = await _repository.GetAllAsync(1, 10, CancellationToken.None);

        // Assert
        data.Should().HaveCount(10);
        hits.Should().BeGreaterOrEqualTo(15);
    }

    [Test]
    public async Task GetByIdAsync_WithDestination_ShouldReturnDestinationWithAttractions()
    {
        // Arrange
        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _repository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(destination.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().Be("Test Address");
        result.Attractions.Should().NotBeNull();
    }
}
