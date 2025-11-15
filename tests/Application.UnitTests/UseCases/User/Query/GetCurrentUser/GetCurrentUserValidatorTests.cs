using Application.UseCases.v1.User.Query.GetCurrentUser;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Query.GetCurrentUser;

[TestFixture]
public class GetCurrentUserValidatorTests
{
    private GetCurrentUserValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new GetCurrentUserValidator();
    }

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        // Arrange
        var query = new GetCurrentUserQuery
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyUserId_ShouldReturnInvalid()
    {
        // Arrange
        var query = new GetCurrentUserQuery
        {
            UserId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
