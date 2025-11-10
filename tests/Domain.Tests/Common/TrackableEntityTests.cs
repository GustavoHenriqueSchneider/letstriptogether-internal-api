using Domain.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.Tests.Common;

[TestFixture]
public class TrackableEntityTests
{
    [Test]
    public void CreatedAt_Getter_ShouldReturnUtcTime()
    {
        // Arrange
        var entity = new TestTrackableEntity();

        // Act
        var createdAt = entity.CreatedAt;

        // Assert
        createdAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        createdAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void UpdatedAt_Getter_WhenNotSet_ShouldReturnNull()
    {
        // Arrange
        var entity = new TestTrackableEntity();

        // Act
        var updatedAt = entity.UpdatedAt;

        // Assert
        updatedAt.Should().BeNull();
    }

    [Test]
    public void SetUpdateAt_ShouldSetUpdatedAtToUtcNow()
    {
        // Arrange
        var entity = new TestTrackableEntity();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        entity.SetUpdateAt();
        var afterUpdate = DateTime.UtcNow;

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt!.Value.Should().BeAfter(beforeUpdate.AddSeconds(-1));
        entity.UpdatedAt.Value.Should().BeBefore(afterUpdate.AddSeconds(1));
        entity.UpdatedAt.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void CreatedAt_Setter_ShouldSetPrivateField()
    {
        // Arrange
        var entity = new TestTrackableEntity();
        var customDate = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act - Use reflection to set private setter
        var createdAtProperty = typeof(TrackableEntity).GetProperty("CreatedAt");
        createdAtProperty!.SetValue(entity, customDate);

        // Assert
        entity.CreatedAt.Should().Be(customDate);
    }

    [Test]
    public void UpdatedAt_Setter_ShouldSetPrivateField()
    {
        // Arrange
        var entity = new TestTrackableEntity();
        var customDate = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act - Use reflection to set private setter
        var updatedAtProperty = typeof(TrackableEntity).GetProperty("UpdatedAt");
        updatedAtProperty!.SetValue(entity, customDate);

        // Assert
        entity.UpdatedAt.Should().Be(customDate);
    }

    [Test]
    public void Id_ShouldBeGenerated()
    {
        // Arrange & Act
        var entity1 = new TestTrackableEntity();
        var entity2 = new TestTrackableEntity();

        // Assert
        entity1.Id.Should().NotBeEmpty();
        entity2.Id.Should().NotBeEmpty();
        entity1.Id.Should().NotBe(entity2.Id);
    }

    private class TestTrackableEntity : TrackableEntity
    {
    }
}
