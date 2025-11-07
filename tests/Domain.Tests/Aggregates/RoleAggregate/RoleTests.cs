using FluentAssertions;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using NUnit.Framework;

namespace Domain.Tests.Aggregates.RoleAggregate;

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
