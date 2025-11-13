using FluentAssertions;
using Infrastructure.Configurations;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Configurations;

[TestFixture]
public class EmailSettingsTests
{
    [Test]
    public void EmailSettings_WithProperties_ShouldSetValues()
    {
        // Arrange & Act
        var settings = new EmailSettings
        {
            SmtpServer = "smtp.example.com",
            Port = 587,
            Username = "user@example.com",
            Password = "password123",
            EnableSsl = true
        };

        // Assert
        settings.SmtpServer.Should().Be("smtp.example.com");
        settings.Port.Should().Be(587);
        settings.Username.Should().Be("user@example.com");
        settings.Password.Should().Be("password123");
        settings.EnableSsl.Should().BeTrue();
    }

    [Test]
    public void EmailSettings_WithDifferentValues_ShouldSetCorrectly()
    {
        // Arrange & Act
        var settings = new EmailSettings
        {
            SmtpServer = "mail.test.com",
            Port = 25,
            Username = "test@test.com",
            Password = "testpass",
            EnableSsl = false
        };

        // Assert
        settings.SmtpServer.Should().Be("mail.test.com");
        settings.Port.Should().Be(25);
        settings.Username.Should().Be("test@test.com");
        settings.Password.Should().Be("testpass");
        settings.EnableSsl.Should().BeFalse();
    }
}
