using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Configurations;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Infrastructure.Tests.Services;

[TestFixture]
public class TokenServiceTests
{
    private TokenService _service = null!;
    private JsonWebTokenSettings _jwtSettings = null!;

    [SetUp]
    public void SetUp()
    {
        _jwtSettings = new JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440,
            ResetPasswordTokenValidityInMinutes = 30,
            InvitationTokenValidityInMinutes = 1440
        };

        _service = new TokenService(Options.Create(_jwtSettings), new JwtSecurityTokenHandler());
    }

    [Test]
    public void GenerateTokens_WithUser_ShouldReturnAccessAndRefreshTokens()
    {
        // Arrange
        var role = new Role { Name = Roles.User };
        var user = new User("Test User", "test@example.com", "hash", role);

        // Act
        var (accessToken, refreshToken) = _service.GenerateTokens(user);

        // Assert
        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void GenerateResetPasswordToken_WithUserId_ShouldReturnToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var token = _service.GenerateResetPasswordToken(userId);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void GenerateInvitationToken_WithInvitationId_ShouldReturnToken()
    {
        // Arrange
        var invitationId = Guid.NewGuid();

        // Act
        var token = _service.GenerateInvitationToken(invitationId);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void GenerateRegisterTokenForStep_WithStepAndClaims_ShouldReturnToken()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(Claims.Name, "Test User"),
            new(ClaimTypes.Email, "test@example.com")
        };

        // Act
        var token = _service.GenerateRegisterTokenForStep(LetsTripTogether.InternalApi.Domain.ValueObjects.Step.ValidateEmail, claims);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void ValidateRefreshToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role { Name = Roles.User };
        var user = new User("Test User", "test@example.com", "hash", role);
        var (_, refreshToken) = _service.GenerateTokens(user);

        // Act
        var (isValid, claims) = _service.ValidateRefreshToken(refreshToken);

        // Assert
        isValid.Should().BeTrue();
        claims.Should().NotBeNull();
    }

    [Test]
    public void ValidateRefreshToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        const string invalidToken = "invalid.token.here";

        // Act
        var (isValid, claims) = _service.ValidateRefreshToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
        claims.Should().BeNull();
    }

    [Test]
    public void IsTokenExpired_WithValidToken_ShouldReturnFalse()
    {
        // Arrange
        var role = new Role { Name = Roles.User };
        var user = new User("Test User", "test@example.com", "hash", role);
        var (accessToken, _) = _service.GenerateTokens(user);

        // Act
        var (isExpired, expiresIn) = _service.IsTokenExpired(accessToken);

        // Assert
        isExpired.Should().BeFalse();
        expiresIn.Should().NotBeNull();
    }

    [Test]
    public void ValidateInvitationToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var invitationId = Guid.NewGuid();
        var token = _service.GenerateInvitationToken(invitationId);

        // Act
        var (isValid, returnedInvitationId) = _service.ValidateInvitationToken(token);

        // Assert
        isValid.Should().BeTrue();
        returnedInvitationId.Should().Be(invitationId.ToString());
    }

    [Test]
    public void ValidateInvitationToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        const string invalidToken = "invalid.token.here";

        // Act
        var (isValid, invitationId) = _service.ValidateInvitationToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
        invitationId.Should().BeNull();
    }

    [Test]
    public void ValidateInvitationToken_WithRefreshToken_ShouldReturnFalse()
    {
        // Arrange
        var role = new Role { Name = Roles.User };
        var user = new User("Test User", "test@example.com", "hash", role);
        var (_, refreshToken) = _service.GenerateTokens(user);

        // Act
        var (isValid, invitationId) = _service.ValidateInvitationToken(refreshToken);

        // Assert
        isValid.Should().BeFalse();
        invitationId.Should().BeNull();
    }
}
