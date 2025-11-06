using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;

[TestFixture]
public class AdminGetAllGroupInvitationsByGroupIdResponseTests
{
    [Test]
    public void AdminGetAllGroupInvitationsByGroupIdResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllGroupInvitationsByGroupIdResponse
        {
            Data = [],
            Hits = 2
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(2);
    }

    [Test]
    public void AdminGetAllGroupInvitationsByGroupIdResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllGroupInvitationsByGroupIdResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
