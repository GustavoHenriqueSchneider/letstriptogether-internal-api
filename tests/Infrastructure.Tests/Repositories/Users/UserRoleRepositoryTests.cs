using Application.Common.Interfaces.Services;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Infrastructure.Tests.Common;
using NUnit.Framework;

namespace Infrastructure.Tests.Repositories.Users;

[TestFixture]
public class UserRoleRepositoryTests : TestBase
{
    private UserRoleRepository _repository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new UserRoleRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task AddAsync_WithValidUserRole_ShouldAdd()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, global::Domain.Security.Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }
        
        var role2 = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.Admin, CancellationToken.None);

        if (role2 is null)
        {
            role2 = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role2, global::Domain.Security.Roles.Admin);
            await _roleRepository.AddAsync(role2, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();
        
        var userRole = new UserRole(user.Id, role2.Id);

        // Act
        await _repository.AddAsync(userRole, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(userRole.Id, CancellationToken.None);
        result.Should().NotBeNull();
    }

    [Test]
    public async Task Remove_WithValidUserRole_ShouldRemove()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, global::Domain.Security.Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }
        
        var role2 = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.Admin, CancellationToken.None);

        if (role2 is null)
        {
            role2 = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role2, global::Domain.Security.Roles.Admin);
            await _roleRepository.AddAsync(role2, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var userRole = new UserRole(user.Id, role2.Id);
        await _repository.AddAsync(userRole, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        _repository.Remove(userRole);
        await DbContext.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(userRole.Id, CancellationToken.None);
        result.Should().BeNull();
    }
}
