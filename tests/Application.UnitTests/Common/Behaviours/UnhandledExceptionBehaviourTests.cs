using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Behaviours;
using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.Common.Behaviours;

[TestFixture]
public class UnhandledExceptionBehaviourTests
{
    private class TestRequest : IRequest<TestResponse>
    {
    }

    private class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }
    
    [Test]
    public async Task Handle_WithNoException_ShouldReturnResponse()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
        var behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        var response = new TestResponse { Result = "Success" };
        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(response);

        // Act
        var result = await behaviour.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
    }

    [Test]
    public void Handle_WithBaseException_ShouldLogWarningAndRethrow()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
        var behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        var exception = new BadRequestException("Test error");
        RequestHandlerDelegate<TestResponse> next = () => throw exception;

        // Act & Assert
        var thrownException = Assert.ThrowsAsync<BadRequestException>(async () =>
            await behaviour.Handle(request, next, CancellationToken.None));

        thrownException.Should().Be(exception);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Test]
    public void Handle_WithDomainBusinessRuleException_ShouldLogWarningAndRethrow()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
        var behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        var exception = new DomainBusinessRuleException("Test error");
        RequestHandlerDelegate<TestResponse> next = () => throw exception;

        // Act & Assert
        var thrownException = Assert.ThrowsAsync<DomainBusinessRuleException>(async () =>
            await behaviour.Handle(request, next, CancellationToken.None));

        thrownException.Should().Be(exception);
    }

    [Test]
    public void Handle_WithGenericException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
        var behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(loggerMock.Object);
        var request = new TestRequest();
        var exception = new InvalidOperationException("Test error");
        RequestHandlerDelegate<TestResponse> next = () => throw exception;

        // Act & Assert
        var thrownException = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await behaviour.Handle(request, next, CancellationToken.None));

        thrownException.Should().Be(exception);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
