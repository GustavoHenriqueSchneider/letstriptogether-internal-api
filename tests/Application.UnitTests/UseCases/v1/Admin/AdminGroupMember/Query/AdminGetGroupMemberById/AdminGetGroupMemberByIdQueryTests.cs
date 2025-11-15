using Application.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

[TestFixture]
public class AdminGetGroupMemberByIdQueryTests
{
    [Test]
    public void AdminGetGroupMemberByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        // Act
        var query = new AdminGetGroupMemberByIdQuery
        {
            GroupId = groupId,
            MemberId = memberId
        };

        // Assert
        query.GroupId.Should().Be(groupId);
        query.MemberId.Should().Be(memberId);
    }
}
