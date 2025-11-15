using Application.UseCases.v1.Invitation.Query.GetInvitationDetails;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Invitation.Query.GetInvitationDetails;

[TestFixture]
public class GetInvitationDetailsValidatorTests
{
    private GetInvitationDetailsValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new GetInvitationDetailsValidator();
    }

    [Test]
    public void Validate_WithValidToken_ShouldBeValid()
    {
        // Arrange
        var query = new GetInvitationDetailsQuery
        {
            Token = "token"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithEmptyToken_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetInvitationDetailsQuery
        {
            Token = string.Empty
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Test]
    public void Validate_WithNullToken_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetInvitationDetailsQuery
        {
            Token = null!
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Token);
    }
}

