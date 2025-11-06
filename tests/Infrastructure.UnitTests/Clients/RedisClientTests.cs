using FluentAssertions;
using LetsTripTogether.InternalApi.Infrastructure.Clients;
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

        // Act
        var client = new RedisClient(connectionString);

        // Assert
        client.Should().NotBeNull();
        client.Database.Should().NotBeNull();
        // Verify Database property is accessible
        var database = client.Database;
        database.Should().NotBeNull();
    }

    [Test]
    public void Database_Property_ShouldBeAccessible()
    {
        // Arrange
        const string connectionString = "localhost:6379,password=admin";
        var client = new RedisClient(connectionString);

        // Act
        var database = client.Database;

        // Assert
        database.Should().NotBeNull();
    }
}
