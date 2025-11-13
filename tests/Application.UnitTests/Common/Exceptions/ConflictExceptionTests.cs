using Application.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions;

[TestFixture]
public class ConflictExceptionTests
{
    [Test]
    public void Constructor_WithMessage_ShouldSetStatusCode409()
    {
        // Arrange
        const string message = "Conflict message";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        exception.Title.Should().Be("Conflict");
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Conflict message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new ConflictException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
