using Domain.Aggregates.RoleAggregate.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.UnitTests.Aggregates.RoleAggregate;

[TestFixture]
public class RoleTests
{
    [Test]
    public void UserRoles_Getter_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var role = new Role();

        // Act
        var userRoles = role.UserRoles;

        // Assert
        userRoles.Should().NotBeNull();
        userRoles.Should().BeEmpty();
    }
}
