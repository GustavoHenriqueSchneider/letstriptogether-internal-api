using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Common.Extensions;

[TestFixture]
public class HttpContextExtensionsTests
{
    [Test]
    public void GetBearerToken_WithBearerToken_ShouldReturnToken()
    {
        // Arrange
        const string token = "test-token-123";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        
        var extensions = new HttpContextExtensions(httpContextAccessor.Object);

        // Act
        var result = extensions.GetBearerToken();

        // Assert
        result.Should().Be(token);
    }

    [Test]
    public void GetBearerToken_WithoutBearerToken_ShouldReturnNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        
        var extensions = new HttpContextExtensions(httpContextAccessor.Object);

        // Act
        var result = extensions.GetBearerToken();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetBearerToken_WithInvalidFormat_ShouldReturnNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "InvalidFormat token";
        
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        
        var extensions = new HttpContextExtensions(httpContextAccessor.Object);

        // Act
        var result = extensions.GetBearerToken();

        // Assert
        result.Should().BeNull();
    }
}
