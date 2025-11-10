using Application.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.Tests.Common.Exceptions;

[TestFixture]
public class NotFoundExceptionTests
{
    [Test]
    public void Constructor_WithMessage_ShouldSetStatusCode404()
    {
        // Arrange
        const string message = "Not found message";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        exception.Title.Should().Be("Not Found");
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Not found message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
