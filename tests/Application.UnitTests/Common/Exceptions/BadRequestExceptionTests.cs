using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions;

[TestFixture]
public class BadRequestExceptionTests
{
    [Test]
    public void Constructor_WithMessage_ShouldSetStatusCode400()
    {
        // Arrange
        const string message = "Bad request message";

        // Act
        var exception = new BadRequestException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Title.Should().Be("Bad Request");
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Bad request message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new BadRequestException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
