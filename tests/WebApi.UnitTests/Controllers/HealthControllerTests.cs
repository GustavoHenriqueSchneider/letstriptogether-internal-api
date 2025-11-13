using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

[TestFixture]
public class HealthControllerTests
{
    private HealthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new HealthController();
    }

    [Test]
    public void Get_ShouldReturnOkWithHealthStatus()
    {
        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        var value = okResult.Value;
        value!.GetType().GetProperty("status")!.GetValue(value).Should().Be("healthy");
        value.GetType().GetProperty("timestamp")!.GetValue(value).Should().NotBeNull();
    }
}
