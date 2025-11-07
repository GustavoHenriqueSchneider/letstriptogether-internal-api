using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMember.Command.RemoveGroupMemberById;
using NUnit.Framework;

namespace Application.Tests.UseCases.GroupMember.Command.RemoveGroupMemberById;

[TestFixture]
public class RemoveGroupMemberByIdValidatorTests
{
    private RemoveGroupMemberByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new RemoveGroupMemberByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new RemoveGroupMemberByIdCommand { GroupId = Guid.NewGuid(), MemberId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
