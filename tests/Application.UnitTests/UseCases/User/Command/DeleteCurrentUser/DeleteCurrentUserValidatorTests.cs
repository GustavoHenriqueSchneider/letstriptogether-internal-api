using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Command.DeleteCurrentUser;

[TestFixture]
public class DeleteCurrentUserValidatorTests
{
    private DeleteCurrentUserValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new DeleteCurrentUserValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new DeleteCurrentUserCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyUserId_ShouldReturnInvalid()
    {
        // Arrange
        var command = new DeleteCurrentUserCommand
        {
            UserId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
