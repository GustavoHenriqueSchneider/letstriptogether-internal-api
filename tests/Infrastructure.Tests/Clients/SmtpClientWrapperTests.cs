using System.Net.Mail;
using FluentAssertions;
using Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Infrastructure.Tests.Clients;

[TestFixture]
public class SmtpClientWrapperTests
{
    [Test]
    public void Constructor_WithSmtpClient_ShouldCreateInstance()
    {
        // Arrange
        using var smtpClient = new SmtpClient();
        var loggerMock = new Mock<ILogger<SmtpClientWrapper>>();

        // Act
        var wrapper = new SmtpClientWrapper(smtpClient, loggerMock.Object);

        // Assert
        wrapper.Should().NotBeNull();
    }

    [Test]
    public async Task SendMailAsync_WithMessage_ShouldDelegateToSmtpClient()
    {
        // Arrange
        using var smtpClient = new SmtpClient
        {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
        };
        
        Directory.CreateDirectory(smtpClient.PickupDirectoryLocation);
        
        var loggerMock = new Mock<ILogger<SmtpClientWrapper>>();
        var wrapper = new SmtpClientWrapper(smtpClient, loggerMock.Object);
        using var message = new MailMessage("from@test.com", "to@test.com", "Subject", "Body");
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        var act = async () => await wrapper.SendMailAsync(message, cancellationToken);
        await act.Should().NotThrowAsync();
        
        // Cleanup
        try
        {
            Directory.Delete(smtpClient.PickupDirectoryLocation, true);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
