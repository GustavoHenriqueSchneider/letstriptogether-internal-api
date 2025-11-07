using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Application.Tests.Common.Exceptions;

[TestFixture]
public class ValidationExceptionTests
{
    [Test]
    public void Constructor_WithErrors_ShouldSetProperties()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } },
            { "Password", new[] { "Password is too short" } }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        exception.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        exception.Title.Should().Be("Validation Error");
        exception.Message.Should().Be("One or more validation errors occurred.");
        exception.Errors.Should().BeEquivalentTo(errors);
    }

    [Test]
    public void Constructor_WithErrorsAndInnerException_ShouldSetInnerException()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };
        var innerException = new Exception("Inner");

        // Act
        var exception = new ValidationException(errors, innerException);

        // Assert
        exception.InnerException.Should().Be(innerException);
        exception.Errors.Should().BeEquivalentTo(errors);
    }

    [Test]
    public void Errors_ShouldBeReadOnly()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };
        var exception = new ValidationException(errors);

        // Act & Assert
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().HaveCount(1);
    }
}
