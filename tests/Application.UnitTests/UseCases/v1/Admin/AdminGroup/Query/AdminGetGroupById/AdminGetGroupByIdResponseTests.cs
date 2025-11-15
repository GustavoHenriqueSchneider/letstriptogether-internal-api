using Application.UseCases.v1.Admin.AdminGroup.Query.AdminGetGroupById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroup.Query.AdminGetGroupById;

[TestFixture]
public class AdminGetGroupByIdResponseTests
{
    [Test]
    public void AdminGetGroupByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var tripExpectedDate = DateTime.UtcNow.AddDays(30);
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);
        var preferences = new AdminGetGroupByIdPreferenceResponse
        {
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = ["Museums"],
            Entertainment = ["Theater"],
            PlaceTypes = ["Restaurant"]
        };

        // Act
        var response = new AdminGetGroupByIdResponse
        {
            Name = "Test Group",
            TripExpectedDate = tripExpectedDate,
            Preferences = preferences,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.Name.Should().Be("Test Group");
        response.TripExpectedDate.Should().Be(tripExpectedDate);
        response.Preferences.Should().NotBeNull();
        response.Preferences.LikesShopping.Should().BeTrue();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetGroupByIdResponse_WithNullUpdatedAt_ShouldSetProperties()
    {
        // Arrange
        var tripExpectedDate = DateTime.UtcNow.AddDays(30);
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetGroupByIdResponse
        {
            Name = "Test Group",
            TripExpectedDate = tripExpectedDate,
            Preferences = new AdminGetGroupByIdPreferenceResponse(),
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.Name.Should().Be("Test Group");
        response.TripExpectedDate.Should().Be(tripExpectedDate);
        response.Preferences.Should().NotBeNull();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }

    [Test]
    public void AdminGetGroupByIdPreferenceResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var preferences = new AdminGetGroupByIdPreferenceResponse
        {
            LikesShopping = false,
            LikesGastronomy = false,
            Culture = ["Museums", "Art"],
            Entertainment = ["Theater", "Cinema"],
            PlaceTypes = ["Restaurant", "Cafe"]
        };

        // Assert
        preferences.LikesShopping.Should().BeFalse();
        preferences.LikesGastronomy.Should().BeFalse();
        preferences.Culture.Should().HaveCount(2);
        preferences.Entertainment.Should().HaveCount(2);
        preferences.PlaceTypes.Should().HaveCount(2);
    }
}
