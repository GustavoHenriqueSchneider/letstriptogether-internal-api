using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;

[TestFixture]
public class AdminGetGroupDestinationVoteByIdResponseTests
{
    [Test]
    public void AdminGetGroupDestinationVoteByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var response = new AdminGetGroupDestinationVoteByIdResponse
        {
            MemberId = memberId,
            DestinationId = destinationId,
            IsApproved = true,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.MemberId.Should().Be(memberId);
        response.DestinationId.Should().Be(destinationId);
        response.IsApproved.Should().BeTrue();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetGroupDestinationVoteByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetGroupDestinationVoteByIdResponse
        {
            MemberId = memberId,
            DestinationId = destinationId,
            IsApproved = false,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.MemberId.Should().Be(memberId);
        response.DestinationId.Should().Be(destinationId);
        response.IsApproved.Should().BeFalse();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }
}
