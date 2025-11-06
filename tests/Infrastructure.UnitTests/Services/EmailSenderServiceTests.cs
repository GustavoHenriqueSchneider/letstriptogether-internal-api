using System.Net.Mail;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Services;

[TestFixture]
public class EmailSenderServiceTests
{
    private EmailSenderService _service = null!;
    private Mock<SmtpClient> _smtpClientMock = null!;

    [SetUp]
    public void SetUp()
    {
        _smtpClientMock = new Mock<SmtpClient>();
        _service = new EmailSenderService(_smtpClientMock.Object);
    }

    [Test]
    public async Task SendAsync_WithValidParameters_ShouldCallSmtpClient()
    {
        // Arrange
        const string to = "test@example.com";
        const string subject = "Test Subject";
        const string body = "Test Body";

        _smtpClientMock.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendAsync(to, subject, body, CancellationToken.None);

        // Assert
        _smtpClientMock.Verify(x => x.SendMailAsync(
            It.Is<MailMessage>(m => m.To[0].Address == to && m.Subject == subject && m.Body == body),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
