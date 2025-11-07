using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;

[TestFixture]
public class AdminRemoveGroupMemberByIdCommandTests
{
    [Test]
    public void AdminRemoveGroupMemberByIdCommand_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        // Act
        var command = new AdminRemoveGroupMemberByIdCommand
        {
            GroupId = groupId,
            MemberId = memberId
        };

        // Assert
        command.GroupId.Should().Be(groupId);
        command.MemberId.Should().Be(memberId);
    }
}
