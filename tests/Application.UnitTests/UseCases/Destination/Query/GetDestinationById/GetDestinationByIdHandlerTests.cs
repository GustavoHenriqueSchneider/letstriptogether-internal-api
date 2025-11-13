using Application.Common.Exceptions;
using Application.UnitTests.Common;
using Application.UseCases.Destination.Query.GetDestinationById;
using Domain.Aggregates.DestinationAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Destinations;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Destination.Query.GetDestinationById;

[TestFixture]
public class GetDestinationByIdHandlerTests : TestBase
{
    private GetDestinationByIdHandler _handler = null!;
    private DestinationRepository _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        _repository = new DestinationRepository(DbContext);
        _handler = new GetDestinationByIdHandler(_repository);
    }

    [Test]
    public async Task Handle_WithValidDestinationId_ShouldReturnDestination()
    {
        // Arrange
        var destination = new Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _repository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetDestinationByIdQuery { DestinationId = destination.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Place.Should().Be("Test Address");
        result.Description.Should().Be("Test Description");
    }

    [Test]
    public async Task Handle_WithInvalidDestinationId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetDestinationByIdQuery { DestinationId = TestDataHelper.GenerateRandomGuid() };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task Handle_WithDestinationWithAttractions_ShouldMapAttractionsCorrectly()
    {
        // Arrange
        var destination = new Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _repository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var attraction1 = new DestinationAttraction
        {
            DestinationId = destination.Id,
            Destination = destination,
            Name = "Attraction 1",
            Description = "Description 1",
            Category = "Food.Restaurant"
        };
        var attraction2 = new DestinationAttraction
        {
            DestinationId = destination.Id,
            Destination = destination,
            Name = "Attraction 2",
            Description = "Description 2",
            Category = "Culture.Museum"
        };

        DbContext.Set<DestinationAttraction>().Add(attraction1);
        DbContext.Set<DestinationAttraction>().Add(attraction2);
        await DbContext.SaveChangesAsync();

        var query = new GetDestinationByIdQuery { DestinationId = destination.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Attractions.Should().NotBeNull();
        result.Attractions.Should().HaveCount(2);
        result.Attractions.Should().Contain(a => a.Name == "Attraction 1" && a.Description == "Description 1" && a.Category == "Food.Restaurant");
        result.Attractions.Should().Contain(a => a.Name == "Attraction 2" && a.Description == "Description 2" && a.Category == "Culture.Museum");
    }
}
