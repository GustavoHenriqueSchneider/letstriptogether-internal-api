using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Tests.Common;
using NUnit.Framework;
using RoleType = Domain.Security.Roles;

namespace Infrastructure.Tests.Repositories.Users;

[TestFixture]
public class UserRepositoryTests : TestBase
{
    private UserRepository _repository = null!;
    private RoleRepository _roleRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        _repository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(RoleType.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, "hash", role);
        await _repository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByEmailAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsByEmailAsync("nonexistent@example.com", CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(RoleType.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var randomEmail = $"test{Guid.NewGuid().ToString()[..8]}@example.com";
        var user = new User("Test User", randomEmail, "hash", role);
        await _repository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(randomEmail, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(randomEmail);
    }

    [Test]
    public async Task GetUserWithRolesByEmailAsync_WithExistingEmail_ShouldReturnUserWithRoles()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(RoleType.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, "hash", role);
        await _repository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserWithRolesByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserRoles.Should().NotBeEmpty();
    }
}
