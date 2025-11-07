using System.Net.Mail;
using FluentAssertions;
using LetsTripTogether.InternalApi.Infrastructure.Clients;
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

        // Act
        var wrapper = new SmtpClientWrapper(smtpClient);

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
        
        var wrapper = new SmtpClientWrapper(smtpClient);
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
