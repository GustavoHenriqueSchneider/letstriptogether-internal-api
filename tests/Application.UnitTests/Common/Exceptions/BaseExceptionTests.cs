using Application.Common.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions;

[TestFixture]
public class BaseExceptionTests
{
    [Test]
    public void Constructor_WithMessage_ShouldSetProperties()
    {
        // Arrange
        const string message = "Test message";
        const int statusCode = 400;

        // Act
        var exception = new BadRequestException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(statusCode);
        exception.Title.Should().Be("Bad Request");
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        // Arrange
        const string message = "Test message";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new BadRequestException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}
