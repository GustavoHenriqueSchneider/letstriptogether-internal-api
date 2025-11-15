using Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

[TestFixture]
public class AdminGetGroupInvitationByIdResponseTests
{
    [Test]
    public void AdminGetGroupInvitationByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var expirationDate = DateTime.UtcNow.AddDays(7);
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);

        // Act
        var response = new AdminGetGroupInvitationByIdResponse
        {
            Status = "Active",
            ExpirationDate = expirationDate,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.Status.Should().Be("Active");
        response.ExpirationDate.Should().Be(expirationDate);
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetGroupInvitationByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var expirationDate = DateTime.UtcNow.AddDays(7);
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetGroupInvitationByIdResponse
        {
            Status = "Expired",
            ExpirationDate = expirationDate,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.Status.Should().Be("Expired");
        response.ExpirationDate.Should().Be(expirationDate);
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }
}
