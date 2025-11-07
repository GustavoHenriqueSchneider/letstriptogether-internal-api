using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.Tests.Common.Exceptions;

[TestFixture]
public class InternalServerErrorExceptionTests
{
    [Test]
    public void Constructor_WithMessage_ShouldSetStatusCode500()
    {
        // Arrange
        const string message = "Internal server error message";

        // Act
        var exception = new InternalServerErrorException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        exception.Title.Should().Be("Internal Server Error");
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Internal server error message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new InternalServerErrorException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
