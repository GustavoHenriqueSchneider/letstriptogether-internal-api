using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.Tests.Aggregates.UserAggregate;

[TestFixture]
public class UserTests
{
    [Test]
    public void AddRole_WithNewRole_ShouldAddRole()
    {
        // Arrange
        var role1 = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role1, Roles.User);
        typeof(Role).GetProperty("Id")!.SetValue(role1, Guid.NewGuid());

        var role2 = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role2, Roles.Admin);
        typeof(Role).GetProperty("Id")!.SetValue(role2, Guid.NewGuid());

        var email = $"test{Guid.NewGuid():N}@example.com";
        var passwordHash = "hashedPassword123";
        var userName = "Test User";

        var user = new User(userName, email, passwordHash, role1);

        // Act - Use reflection to call private AddRole method
        var addRoleMethod = typeof(User).GetMethod("AddRole", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        addRoleMethod!.Invoke(user, new object[] { role2 });

        // Assert
        user.UserRoles.Should().HaveCount(2);
        user.UserRoles.Should().Contain(ur => ur.RoleId == role1.Id);
        user.UserRoles.Should().Contain(ur => ur.RoleId == role2.Id);
    }

    [Test]
    public void AddRole_WithDuplicateRole_ShouldNotAddDuplicate()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        typeof(Role).GetProperty("Id")!.SetValue(role, Guid.NewGuid());

        var email = $"test{Guid.NewGuid():N}@example.com";
        var passwordHash = "hashedPassword123";
        var userName = "Test User";

        var user = new User(userName, email, passwordHash, role);

        // Act - Try to add the same role again
        var addRoleMethod = typeof(User).GetMethod("AddRole", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        addRoleMethod!.Invoke(user, new object[] { role });

        // Assert
        user.UserRoles.Should().HaveCount(1);
        user.UserRoles.Should().Contain(ur => ur.RoleId == role.Id);
    }

    [Test]
    public void Constructor_WithRole_ShouldAddRole()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        typeof(Role).GetProperty("Id")!.SetValue(role, Guid.NewGuid());

        var email = $"test{Guid.NewGuid():N}@example.com";
        var passwordHash = "hashedPassword123";
        var userName = "Test User";

        // Act
        var user = new User(userName, email, passwordHash, role);

        // Assert
        user.UserRoles.Should().HaveCount(1);
        user.UserRoles.Should().Contain(ur => ur.RoleId == role.Id);
        user.Name.Should().Be(userName);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
    }
}
