using System.Net;
using System.Text.Json;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Services;

[TestFixture]
public class NotificationServiceTests
{
    private NotificationService _service = null!;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
    private HttpClient _httpClient = null!;
    private Mock<ILogger<NotificationService>> _loggerMock = null!;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.example.com")
        };
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<NotificationService>>();
        _service = new NotificationService(_httpClient, _httpContextAccessorMock.Object, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task SendNotificationAsync_WithValidParameters_ShouldCallHttpClient()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string eventName = "test_event";
        var data = new { testProperty = "testValue" };
        var cancellationToken = CancellationToken.None;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });

        // Act
        await _service.SendNotificationAsync(userId, eventName, data, cancellationToken);

        // Assert
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public async Task SendNotificationAsync_WhenHttpClientThrowsException_ShouldLogError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string eventName = "test_event";
        var data = new { testProperty = "testValue" };
        var cancellationToken = CancellationToken.None;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        await _service.SendNotificationAsync(userId, eventName, data, cancellationToken);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Test]
    public async Task SendNotificationAsync_WhenHttpClientReturnsErrorStatusCode_ShouldLogError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string eventName = "test_event";
        var data = new { testProperty = "testValue" };
        var cancellationToken = CancellationToken.None;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Server Error")
            });

        // Act
        await _service.SendNotificationAsync(userId, eventName, data, cancellationToken);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Test]
    public async Task SendNotificationAsync_WithValidParameters_ShouldSendCorrectRequestData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string eventName = "group_match_created";
        var data = new { groupId = "123e4567-e89b-12d3-a456-426614174000" };
        var cancellationToken = CancellationToken.None;
        HttpRequestMessage? capturedRequest = null;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, ct) =>
            {
                capturedRequest = req;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });

        // Act
        await _service.SendNotificationAsync(userId, eventName, data, cancellationToken);

        // Assert
        capturedRequest.Should().NotBeNull();
        var jsonContent = await capturedRequest!.Content!.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<NotificationRequest>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        content.Should().NotBeNull();
        content!.UserId.Should().Be(userId);
        content.EventName.Should().Be(eventName);
        content.Data.Should().NotBeNull();
        content.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    private class NotificationRequest
    {
        public Guid UserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public JsonElement Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
