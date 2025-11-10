using Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

[TestFixture]
public class AdminRemoveGroupMemberByIdValidatorTests
{
    private AdminRemoveGroupMemberByIdValidator _validator = null!;
    [SetUp] public void SetUp() => _validator = new AdminRemoveGroupMemberByIdValidator();
    [Test]
    public void Validate_WithValidCommand_ShouldReturnValid()
    {
        var command = new AdminRemoveGroupMemberByIdCommand { GroupId = Guid.NewGuid(), MemberId = Guid.NewGuid() };
        _validator.Validate(command).IsValid.Should().BeTrue();
    }
}
