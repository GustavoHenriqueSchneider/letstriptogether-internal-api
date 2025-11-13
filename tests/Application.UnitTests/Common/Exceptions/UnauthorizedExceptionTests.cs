using Application.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions;

[TestFixture]
public class UnauthorizedExceptionTests
{
    [Test]
    public void Constructor_WithDefaultMessage_ShouldSetStatusCode401()
    {
        // Act
        var exception = new UnauthorizedException();

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        exception.Title.Should().Be("Unauthorized");
        exception.Message.Should().Be("Unauthorized");
    }

    [Test]
    public void Constructor_WithCustomMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Custom unauthorized message";

        // Act
        var exception = new UnauthorizedException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Unauthorized message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new UnauthorizedException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
