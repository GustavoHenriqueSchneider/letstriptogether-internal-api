using Application.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

[TestFixture]
public class AdminGetGroupMemberAllDestinationVotesByIdResponseTests
{
    [Test]
    public void AdminGetGroupMemberAllDestinationVotesByIdResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetGroupMemberAllDestinationVotesByIdResponse
        {
            Data = [],
            Hits = 6
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(6);
    }

    [Test]
    public void AdminGetGroupMemberAllDestinationVotesByIdResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetGroupMemberAllDestinationVotesByIdResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
