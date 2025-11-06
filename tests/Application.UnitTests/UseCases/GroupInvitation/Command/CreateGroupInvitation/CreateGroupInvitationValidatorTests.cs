using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupInvitation.Command.CreateGroupInvitation;

[TestFixture]
public class CreateGroupInvitationValidatorTests
{
    private CreateGroupInvitationValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new CreateGroupInvitationValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new CreateGroupInvitationCommand { GroupId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
