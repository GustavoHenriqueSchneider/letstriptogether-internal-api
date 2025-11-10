using Application.UseCases.Auth.Command.RefreshToken;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.RefreshToken;

[TestFixture]
public class RefreshTokenValidatorTests
{
    private RefreshTokenValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new RefreshTokenValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new RefreshTokenCommand
        {
            RefreshToken = "valid-token"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyToken_ShouldReturnInvalid()
    {
        // Arrange
        var command = new RefreshTokenCommand
        {
            RefreshToken = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
