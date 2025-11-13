using Application.UnitTests.Common;
using Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;
using FluentAssertions;
using Infrastructure.Repositories.Destinations;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;

[TestFixture]
public class AdminGetAllDestinationsHandlerTests : TestBase
{
    private AdminGetAllDestinationsHandler _handler = null!;
    private DestinationRepository _destinationRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _destinationRepository = new DestinationRepository(DbContext);
        _handler = new AdminGetAllDestinationsHandler(_destinationRepository);
    }

    [Test]
    public async Task Handle_WithDestinations_ShouldReturnPaginatedResults()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var destination = new Domain.Aggregates.DestinationAggregate.Entities.Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await _destinationRepository.AddAsync(destination, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        var query = new AdminGetAllDestinationsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(10);
        result.Hits.Should().Be(15);
    }
}
