using System;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.UnitTests.Common.Exceptions;

[TestFixture]
public class ForbiddenExceptionTests
{
    [Test]
    public void Constructor_WithDefaultMessage_ShouldSetStatusCode403()
    {
        // Act
        var exception = new ForbiddenException();

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        exception.Title.Should().Be("Forbidden");
        exception.Message.Should().Be("Forbidden");
    }

    [Test]
    public void Constructor_WithCustomMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Custom forbidden message";

        // Act
        var exception = new ForbiddenException(message);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        exception.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        const string message = "Forbidden message";
        var innerException = new Exception("Inner");

        // Act
        var exception = new ForbiddenException(message, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
    }
}
