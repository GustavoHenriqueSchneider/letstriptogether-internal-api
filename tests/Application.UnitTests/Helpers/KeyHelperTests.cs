using System;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Helpers;
using NUnit.Framework;

namespace Application.UnitTests.Helpers;

[TestFixture]
public class KeyHelperTests
{
    [Test]
    public void RegisterEmailConfirmation_WithEmail_ShouldReturnCorrectKey()
    {
        // Arrange
        const string email = "test@example.com";

        // Act
        var result = KeyHelper.RegisterEmailConfirmation(email);

        // Assert
        result.Should().Be($"auth:register:email-confirmation:{email}");
    }

    [Test]
    public void UserRefreshToken_WithUserId_ShouldReturnCorrectKey()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = KeyHelper.UserRefreshToken(userId);

        // Assert
        result.Should().Be($"auth:user:refresh-token:{userId}");
    }

    [Test]
    public void UserResetPassword_WithUserId_ShouldReturnCorrectKey()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = KeyHelper.UserResetPassword(userId);

        // Assert
        result.Should().Be($"auth:user:reset-password:{userId}");
    }
}
