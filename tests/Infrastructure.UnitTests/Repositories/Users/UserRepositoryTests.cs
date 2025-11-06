using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using NUnit.Framework;
using RoleType = LetsTripTogether.InternalApi.Domain.Security.Roles;

namespace Infrastructure.UnitTests.Repositories.Users;

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
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

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
        var role = new Role { Name = "User" };
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var user = new User("Test User", "test@example.com", "hash", role);
        await _repository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Test]
    public async Task GetUserWithRolesByEmailAsync_WithExistingEmail_ShouldReturnUserWithRoles()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, RoleType.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

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
