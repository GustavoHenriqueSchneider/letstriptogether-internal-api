using Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

[TestFixture]
public class AdminGetGroupMemberByIdResponseTests
{
    [Test]
    public void AdminGetGroupMemberByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var response = new AdminGetGroupMemberByIdResponse
        {
            UserId = userId,
            IsOwner = true,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.UserId.Should().Be(userId);
        response.IsOwner.Should().BeTrue();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetGroupMemberByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetGroupMemberByIdResponse
        {
            UserId = userId,
            IsOwner = false,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.UserId.Should().Be(userId);
        response.IsOwner.Should().BeFalse();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }
}
