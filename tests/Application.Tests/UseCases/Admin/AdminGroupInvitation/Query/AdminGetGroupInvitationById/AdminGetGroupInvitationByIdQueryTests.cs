using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

[TestFixture]
public class AdminGetGroupInvitationByIdQueryTests
{
    [Test]
    public void AdminGetGroupInvitationByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();

        // Act
        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = groupId,
            InvitationId = invitationId
        };

        // Assert
        query.GroupId.Should().Be(groupId);
        query.InvitationId.Should().Be(invitationId);
    }
}
