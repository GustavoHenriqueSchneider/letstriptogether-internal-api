using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;
using NUnit.Framework;

namespace Application.Tests.UseCases.Group.Command.CreateGroup;

[TestFixture]
public class CreateGroupValidatorTests
{
    private CreateGroupValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateGroupValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var command = new CreateGroupCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Group",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
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
        var command = new CreateGroupCommand
        {
            UserId = Guid.Empty,
            Name = "Test Group",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithPastDate_ShouldReturnInvalid()
    {
        // Arrange
        var command = new CreateGroupCommand
        {
            UserId = Guid.NewGuid(),
            Name = "Test Group",
            TripExpectedDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Test]
    public void Validate_WithEmptyName_ShouldReturnInvalid()
    {
        // Arrange
        var command = new CreateGroupCommand
        {
            UserId = Guid.NewGuid(),
            Name = "",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
