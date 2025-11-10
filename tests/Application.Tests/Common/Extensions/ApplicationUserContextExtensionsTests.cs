using System.Security.Claims;
using Application.Common.Extensions;
using Domain.Security;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.Common.Extensions;

[TestFixture]
public class ApplicationUserContextExtensionsTests
{
    [Test]
    public void GetId_WithValidIdClaim_ShouldReturnGuid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(Claims.Id, userId.ToString())
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetId();

        // Assert
        result.Should().Be(userId);
    }

    [Test]
    public void GetId_WithoutIdClaim_ShouldReturnEmptyGuid()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetId();

        // Assert
        result.Should().Be(Guid.Empty);
    }

    [Test]
    public void GetName_WithValidNameClaim_ShouldReturnName()
    {
        // Arrange
        const string name = "Test User";
        var claims = new List<Claim>
        {
            new(Claims.Name, name)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetName();

        // Assert
        result.Should().Be(name);
    }

    [Test]
    public void GetName_WithoutNameClaim_ShouldReturnEmptyString()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetName();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetEmail_WithValidEmailClaim_ShouldReturnEmail()
    {
        // Arrange
        const string email = "test@example.com";
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetEmail();

        // Assert
        result.Should().Be(email);
    }

    [Test]
    public void GetEmail_WithoutEmailClaim_ShouldReturnEmptyString()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetEmail();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetRegisterStep_WithValidStepClaim_ShouldReturnStep()
    {
        // Arrange
        const string step = "ValidateEmail";
        var claims = new List<Claim>
        {
            new(Claims.Step, step)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetRegisterStep();

        // Assert
        result.Should().Be(step);
    }

    [Test]
    public void GetRegisterStep_WithoutStepClaim_ShouldReturnEmptyString()
    {
        // Arrange
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var extensions = new ApplicationUserContextExtensions(principal);

        // Act
        var result = extensions.GetRegisterStep();

        // Assert
        result.Should().BeEmpty();
    }
}
