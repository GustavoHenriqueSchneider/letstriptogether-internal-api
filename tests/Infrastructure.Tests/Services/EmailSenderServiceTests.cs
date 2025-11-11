using System.Net.Mail;
using Application.Common.Interfaces.Services;
using Infrastructure.Clients;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Infrastructure.Tests.Services;

[TestFixture]
public class EmailSenderServiceTests
{
    private EmailSenderService _service = null!;
    private Mock<ISmtpClient> _smtpClientMock = null!;
    private Mock<IEmailTemplateService> _templateServiceMock = null!;

    [SetUp]
    public void SetUp()
    {
        var from = "example@example.com";
        _smtpClientMock = new Mock<ISmtpClient>();
        _templateServiceMock = new Mock<IEmailTemplateService>();
        var loggerMock = new Mock<ILogger<EmailSenderService>>();
        _service = new EmailSenderService(from, _smtpClientMock.Object, _templateServiceMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task SendAsync_WithValidParameters_ShouldCallSmtpClient()
    {
        // Arrange
        const string to = "test@example.com";
        const string templateName = "email-confirmation";
        const string subject = "Test Subject";
        const string htmlBody = "<html><body>Test Body</body></html>";
        var templateData = new Dictionary<string, string>
        {
            { "name", "Test User" },
            { "code", "123456" }
        };

        _templateServiceMock.Setup(x => x.GetTemplateAsync(templateName, templateData))
            .ReturnsAsync((subject, htmlBody));

        _smtpClientMock.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendAsync(to, templateName, templateData, CancellationToken.None);

        // Assert
        _templateServiceMock.Verify(x => x.GetTemplateAsync(templateName, templateData), Times.Once);
        _smtpClientMock.Verify(x => x.SendMailAsync(
            It.Is<MailMessage>(m => m.To[0].Address == to && m.Subject == subject && m.Body == htmlBody && m.IsBodyHtml),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
