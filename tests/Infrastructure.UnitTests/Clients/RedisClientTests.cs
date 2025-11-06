using FluentAssertions;
using LetsTripTogether.InternalApi.Infrastructure.Clients;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Clients;

[TestFixture]
public class RedisClientTests
{
    [Test]
    public void Constructor_WithValidConnectionString_ShouldCreateClient()
    {
        // Arrange
        const string connectionString = "localhost:6379";

        // Act
        var client = new RedisClient(connectionString);

        // Assert
        client.Should().NotBeNull();
        client.Database.Should().NotBeNull();
    }
}
