using Application.UseCases.v1.Invitation.Command.RefuseInvitation;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Invitation.Command.RefuseInvitation;

[TestFixture]
public class RefuseInvitationValidatorTests
{
    private RefuseInvitationValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new RefuseInvitationValidator();

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new RefuseInvitationCommand { Token = "test-token", UserId = Guid.NewGuid() };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
