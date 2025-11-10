using Application.UseCases.Invitation.Command.AcceptInvitation;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Invitation.Command.AcceptInvitation;

[TestFixture]
public class AcceptInvitationValidatorTests
{
    private AcceptInvitationValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new AcceptInvitationValidator();

    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AcceptInvitationCommand { Token = "test-token", UserId = Guid.NewGuid() };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
