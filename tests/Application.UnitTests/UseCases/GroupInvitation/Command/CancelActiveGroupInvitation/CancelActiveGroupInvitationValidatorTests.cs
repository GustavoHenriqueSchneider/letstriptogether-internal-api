using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;

[TestFixture]
public class CancelActiveGroupInvitationValidatorTests
{
    private CancelActiveGroupInvitationValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new CancelActiveGroupInvitationValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new CancelActiveGroupInvitationCommand { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
