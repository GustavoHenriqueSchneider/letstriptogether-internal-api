using FluentAssertions;
using Infrastructure.Configurations;
using NUnit.Framework;

namespace Infrastructure.Tests.Configurations;

[TestFixture]
public class JsonWebTokenSettingsTests
{
    [Test]
    public void JsonWebTokenSettings_WithProperties_ShouldSetValues()
    {
        // Arrange & Act
        var settings = new JsonWebTokenSettings
        {
            Audience = "test-audience",
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440,
            ResetPasswordTokenValidityInMinutes = 30,
            InvitationTokenValidityInMinutes = 1440
        };

        // Assert
        settings.Audience.Should().Be("test-audience");
        settings.Issuer.Should().Be("test-issuer");
        settings.SecretKey.Should().Be("test-secret-key-that-is-at-least-32-characters-long");
        settings.AccessTokenValidityInMinutes.Should().Be(60);
        settings.RefreshTokenValidityInMinutes.Should().Be(1440);
        settings.ResetPasswordTokenValidityInMinutes.Should().Be(30);
        settings.InvitationTokenValidityInMinutes.Should().Be(1440);
    }

    [Test]
    public void JsonWebTokenSettings_WithDifferentValues_ShouldSetCorrectly()
    {
        // Arrange & Act
        var settings = new JsonWebTokenSettings
        {
            Audience = "another-audience",
            Issuer = "another-issuer",
            SecretKey = "another-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 120,
            RefreshTokenValidityInMinutes = 2880,
            ResetPasswordTokenValidityInMinutes = 60,
            InvitationTokenValidityInMinutes = 2880
        };

        // Assert
        settings.Audience.Should().Be("another-audience");
        settings.Issuer.Should().Be("another-issuer");
        settings.SecretKey.Should().Be("another-secret-key-that-is-at-least-32-characters-long");
        settings.AccessTokenValidityInMinutes.Should().Be(120);
        settings.RefreshTokenValidityInMinutes.Should().Be(2880);
        settings.ResetPasswordTokenValidityInMinutes.Should().Be(60);
        settings.InvitationTokenValidityInMinutes.Should().Be(2880);
    }
}
