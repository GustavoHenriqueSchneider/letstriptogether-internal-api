using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;
using NUnit.Framework;

namespace Application.Tests.UseCases.Invitation.Command.RefuseInvitation;

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
