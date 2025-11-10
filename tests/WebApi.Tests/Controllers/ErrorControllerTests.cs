using Application.Common.Exceptions;
using Domain.Common.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers;

namespace WebApi.Tests.Controllers;

[TestFixture]
public class ErrorControllerTests
{
    private ErrorController _controller = null!;
    private Mock<IExceptionHandlerPathFeature> _exceptionHandlerFeatureMock = null!;
    private DefaultHttpContext _httpContext = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new ErrorController();
        _httpContext = new DefaultHttpContext();
        _exceptionHandlerFeatureMock = new Mock<IExceptionHandlerPathFeature>();
        _httpContext.Features.Set(_exceptionHandlerFeatureMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Test]
    public void Error_WithBaseException_ShouldReturnAppropriateStatusCode()
    {
        // Arrange
        var exception = new BadRequestException("Bad request error");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public void Error_WithValidationException_ShouldIncludeErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };
        var exception = new ValidationException(errors);
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        var problemDetails = objectResult!.Value as ProblemDetails;
        problemDetails!.Extensions.Should().ContainKey("errors");
    }

    [Test]
    public void Error_WithDomainBusinessRuleException_ShouldReturnAppropriateStatusCode()
    {
        // Arrange
        var exception = new DomainBusinessRuleException("Business rule violation");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Test]
    public void Error_WithGenericException_ShouldReturn500()
    {
        // Arrange
        var exception = new InvalidOperationException("Generic error");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
