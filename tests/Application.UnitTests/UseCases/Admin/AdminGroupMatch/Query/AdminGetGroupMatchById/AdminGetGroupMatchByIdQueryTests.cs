using Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

[TestFixture]
public class AdminGetGroupMatchByIdQueryTests
{
    [Test]
    public void AdminGetGroupMatchByIdQuery_ShouldSetProperties()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var matchId = Guid.NewGuid();

        // Act
        var query = new AdminGetGroupMatchByIdQuery
        {
            GroupId = groupId,
            MatchId = matchId
        };

        // Assert
        query.GroupId.Should().Be(groupId);
        query.MatchId.Should().Be(matchId);
    }
}
