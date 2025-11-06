using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Domain;

[TestFixture]
public class GroupTests : TestBase
{
    [Test]
    public void HasMatch_WithExistingMatch_ShouldReturnTrue()
    {
        // Arrange
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var destinationId = Guid.NewGuid();
        
        // Create a match using reflection to access private _matches list
        var matchesField = typeof(Group).GetField("_matches", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var matchesList = (System.Collections.Generic.List<GroupMatch>)matchesField!.GetValue(group)!;
        
        var match = new GroupMatch
        {
            GroupId = group.Id,
            DestinationId = destinationId
        };
        matchesList.Add(match);

        // Act
        var result = group.HasMatch(match);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasMatch_WithoutMatch_ShouldReturnFalse()
    {
        // Arrange
        var group = new Group("Test Group", DateTime.UtcNow.AddDays(30));
        var match = new GroupMatch
        {
            GroupId = group.Id,
            DestinationId = Guid.NewGuid()
        };

        // Act
        var result = group.HasMatch(match);

        // Assert
        result.Should().BeFalse();
    }
}
