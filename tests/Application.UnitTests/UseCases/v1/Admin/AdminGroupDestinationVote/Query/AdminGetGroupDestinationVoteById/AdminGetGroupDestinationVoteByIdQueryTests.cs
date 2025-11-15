using Application.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

[TestFixture]
public class AdminGetGroupDestinationVoteByIdQueryTests
{
    [Test]
    public void AdminGetGroupDestinationVoteByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var destinationVoteId = Guid.NewGuid();

        // Act
        var query = new AdminGetGroupDestinationVoteByIdQuery
        {
            GroupId = groupId,
            DestinationVoteId = destinationVoteId
        };

        // Assert
        query.GroupId.Should().Be(groupId);
        query.DestinationVoteId.Should().Be(destinationVoteId);
    }
}
