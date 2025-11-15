using Application.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

[TestFixture]
public class AdminGetAllGroupDestinationVotesByIdResponseTests
{
    [Test]
    public void AdminGetAllGroupDestinationVotesByIdResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllGroupDestinationVotesByIdResponse
        {
            Data = [],
            Hits = 3
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(3);
    }

    [Test]
    public void AdminGetAllGroupDestinationVotesByIdResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllGroupDestinationVotesByIdResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
