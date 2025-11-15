using Application.UseCases.Health.Query.GetHealth;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

[TestFixture]
public class HealthControllerTests
{
    private HealthController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new HealthController(_mediatorMock.Object);
    }

    [Test]
    public async Task Get_WhenHealthy_ShouldReturnOkWithHealthStatus()
    {
        // Arrange
        var response = new GetHealthResponse
        {
            Status = "healthy",
            Timestamp = DateTime.UtcNow,
            Checks = new List<HealthCheckData>
            {
                new()
                {
                    Name = "database",
                    Status = "healthy",
                    Description = "Database is available",
                    Duration = 50.0
                },
                new()
                {
                    Name = "redis",
                    Status = "healthy",
                    Description = "Redis is available",
                    Duration = 30.0
                }
            }
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetHealthQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task Get_WhenUnhealthy_ShouldReturnServiceUnavailable()
    {
        // Arrange
        var response = new GetHealthResponse
        {
            Status = "unhealthy",
            Timestamp = DateTime.UtcNow,
            Checks = new List<HealthCheckData>
            {
                new()
                {
                    Name = "database",
                    Status = "unhealthy",
                    Description = "Database connection failed",
                    Duration = 50.0,
                    Exception = "Connection timeout"
                },
                new()
                {
                    Name = "redis",
                    Status = "healthy",
                    Description = "Redis is available",
                    Duration = 30.0
                }
            }
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetHealthQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(503);
        objectResult.Value.Should().NotBeNull();
        objectResult.Value.Should().BeEquivalentTo(response);
    }
}
