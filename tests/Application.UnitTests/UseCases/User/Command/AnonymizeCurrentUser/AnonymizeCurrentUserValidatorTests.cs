using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Command.AnonymizeCurrentUser;

[TestFixture]
public class AnonymizeCurrentUserValidatorTests
{
    private AnonymizeCurrentUserValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new AnonymizeCurrentUserValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new AnonymizeCurrentUserCommand
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
        var command = new AnonymizeCurrentUserCommand
        {
            UserId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
