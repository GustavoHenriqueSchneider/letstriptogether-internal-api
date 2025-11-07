using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;

[TestFixture]
public class AdminGetAllGroupMembersByIdResponseTests
{
    [Test]
    public void AdminGetAllGroupMembersByIdResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllGroupMembersByIdResponse
        {
            Data = [],
            Hits = 5
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(5);
    }

    [Test]
    public void AdminGetAllGroupMembersByIdResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllGroupMembersByIdResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
