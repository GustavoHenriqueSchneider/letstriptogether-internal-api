using FluentAssertions;
using Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Clients;

[TestFixture]
public class RedisClientTests
{
    [Test]
    public void Constructor_WithConnectionString_ShouldSetDatabaseProperty()
    {
        // Arrange
        const string connectionString = "localhost:6379,password=admin";
        var loggerMock = new Mock<ILogger<RedisClient>>();

        // Act
        var client = new RedisClient(connectionString, loggerMock.Object);

        // Assert
        client.Should().NotBeNull();
        client.Database.Should().NotBeNull();
        var database = client.Database;
        database.Should().NotBeNull();
    }

    [Test]
    public void Database_Property_ShouldBeAccessible()
    {
        // Arrange
        const string connectionString = "localhost:6379,password=admin";
        var loggerMock = new Mock<ILogger<RedisClient>>();
        var client = new RedisClient(connectionString, loggerMock.Object);

        // Act
        var database = client.Database;

        // Assert
        database.Should().NotBeNull();
    }
}
