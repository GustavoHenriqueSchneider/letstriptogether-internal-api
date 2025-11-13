using Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;
using FluentAssertions;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Query.AdminGetUserById;

[TestFixture]
public class AdminGetUserByIdResponseTests
{
    [Test]
    public void AdminGetUserByIdResponse_ShouldSetProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddDays(1);
        var preferences = new AdminGetUserByIdPreferenceResponse
        {
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = ["Museums"],
            Entertainment = ["Theater"],
            PlaceTypes = ["Restaurant"]
        };

        // Act
        var response = new AdminGetUserByIdResponse
        {
            Name = "Test User",
            Email = "test@example.com",
            Preferences = preferences,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        // Assert
        response.Name.Should().Be("Test User");
        response.Email.Should().Be("test@example.com");
        response.Preferences.Should().NotBeNull();
        response.Preferences!.LikesShopping.Should().BeTrue();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Test]
    public void AdminGetUserByIdResponse_WithNullPreferences_ShouldSetProperties()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var response = new AdminGetUserByIdResponse
        {
            Name = "Test User",
            Email = "test@example.com",
            Preferences = null,
            CreatedAt = createdAt,
            UpdatedAt = null
        };

        // Assert
        response.Name.Should().Be("Test User");
        response.Email.Should().Be("test@example.com");
        response.Preferences.Should().BeNull();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }

    [Test]
    public void AdminGetUserByIdPreferenceResponse_ShouldSetProperties()
    {
        // Arrange & Act
        var preferences = new AdminGetUserByIdPreferenceResponse
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
