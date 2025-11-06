using FluentAssertions;
using LetsTripTogether.InternalApi.Infrastructure.Clients;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace Infrastructure.UnitTests.Services;

[TestFixture]
public class RedisServiceTests
{
    private RedisService _service = null!;
    private Mock<RedisClient> _redisClientMock = null!;
    private Mock<IDatabase> _databaseMock = null!;

    [SetUp]
    public void SetUp()
    {
        _databaseMock = new Mock<IDatabase>();
        _redisClientMock = new Mock<RedisClient>("localhost:6379");
        _redisClientMock.Setup(x => x.Database).Returns(_databaseMock.Object);
        
        _service = new RedisService(_redisClientMock.Object);
    }

    [Test]
    public async Task GetAsync_WithKey_ShouldCallDatabase()
    {
        // Arrange
        const string key = "test-key";
        const string value = "test-value";
        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(value);

        // Act
        var result = await _service.GetAsync(key);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public async Task SetAsync_WithKeyAndValue_ShouldCallDatabase()
    {
        // Arrange
        const string key = "test-key";
        const string value = "test-value";
        const int ttlSeconds = 60;

        _databaseMock.Setup(x => x.StringSetAsync(key, value, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _service.SetAsync(key, value, ttlSeconds);

        // Assert
        _databaseMock.Verify(x => x.StringSetAsync(
            key, 
            value, 
            It.Is<TimeSpan>(ts => ts.TotalSeconds == ttlSeconds),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WithKey_ShouldCallDatabase()
    {
        // Arrange
        const string key = "test-key";
        _databaseMock.Setup(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(key);

        // Assert
        _databaseMock.Verify(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }
}
