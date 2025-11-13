using Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

[TestFixture]
public class AdminGetAllGroupMatchesByIdResponseTests
{
    [Test]
    public void AdminGetAllGroupMatchesByIdResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var response = new AdminGetAllGroupMatchesByIdResponse
        {
            Data = [],
            Hits = 4
        };

        // Assert
        response.Data.Should().BeEmpty();
        response.Hits.Should().Be(4);
    }

    [Test]
    public void AdminGetAllGroupMatchesByIdResponseData_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var data = new AdminGetAllGroupMatchesByIdResponseData
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Assert
        data.Id.Should().Be(id);
        data.CreatedAt.Should().Be(createdAt);
    }
}
