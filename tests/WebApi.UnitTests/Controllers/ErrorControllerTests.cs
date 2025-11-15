using Application.Common.Exceptions;
using Application.UseCases.Error.Query.GetError;
using Domain.Common.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

[TestFixture]
public class ErrorControllerTests
{
    private ErrorController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IExceptionHandlerPathFeature> _exceptionHandlerFeatureMock = null!;
    private DefaultHttpContext _httpContext = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ErrorController(_mediatorMock.Object);
        _httpContext = new DefaultHttpContext();
        _exceptionHandlerFeatureMock = new Mock<IExceptionHandlerPathFeature>();
        _httpContext.Features.Set(_exceptionHandlerFeatureMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Test]
    public async Task Error_WithBaseException_ShouldReturnAppropriateStatusCode()
    {
        // Arrange
        var exception = new BadRequestException("Bad request error");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        var response = new GetErrorResponse
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = "Bad request error",
            Instance = "/api/test"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetErrorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task Error_WithValidationException_ShouldIncludeErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } }
        };
        var exception = new ValidationException(errors);
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        var response = new GetErrorResponse
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred",
            Instance = "/api/test",
            Extensions = new Dictionary<string, object> { { "errors", errors } }
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetErrorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        var responseValue = objectResult!.Value as GetErrorResponse;
        responseValue!.Extensions.Should().ContainKey("errors");
    }

    [Test]
    public async Task Error_WithDomainBusinessRuleException_ShouldReturnAppropriateStatusCode()
    {
        // Arrange
        var exception = new DomainBusinessRuleException("Business rule violation");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        var response = new GetErrorResponse
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Business Rule Violation",
            Detail = "Business rule violation",
            Instance = "/api/test"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetErrorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Test]
    public async Task Error_WithGenericException_ShouldReturn500()
    {
        // Arrange
        var exception = new InvalidOperationException("Generic error");
        _exceptionHandlerFeatureMock.Setup(x => x.Error).Returns(exception);
        _exceptionHandlerFeatureMock.Setup(x => x.Path).Returns("/api/test");

        var response = new GetErrorResponse
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request",
            Detail = "Generic error",
            Instance = "/api/test"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetErrorQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Error();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
