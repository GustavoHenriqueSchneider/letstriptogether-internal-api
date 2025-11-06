using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Destinations;
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
        var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
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
    public void Handle_WithInvalidDestinationId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetDestinationByIdQuery { DestinationId = TestDataHelper.GenerateRandomGuid() };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
