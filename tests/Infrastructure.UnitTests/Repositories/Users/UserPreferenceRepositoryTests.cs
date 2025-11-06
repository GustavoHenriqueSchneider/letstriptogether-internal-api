using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories.Users;

[TestFixture]
public class UserPreferenceRepositoryTests : TestBase
{
    private UserPreferenceRepository _repository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new UserPreferenceRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task AddAsync_WithNewPreference_ShouldAddPreference()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { "Italian" },
            culture: new List<string> { "Museums" },
            entertainment: new List<string> { "Nightlife" },
            placeTypes: new List<string> { "Beach" });
        
        user.SetPreferences(preferences);
        _userRepository.Update(user);
        await DbContext.SaveChangesAsync();

        // Act
        await _repository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.Food.Should().HaveCount(1);
    }

    [Test]
    public async Task AddOrUpdateAsync_WithExistingPreference_ShouldUpdatePreference()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences1 = new UserPreference(
            likesCommercial: true,
            food: new List<string> { "Italian" },
            culture: new List<string> { "Museums" },
            entertainment: new List<string> { "Nightlife" },
            placeTypes: new List<string> { "Beach" });
        
        user.SetPreferences(preferences1);
        _userRepository.Update(user);
        await DbContext.SaveChangesAsync();
        await _repository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences2 = new UserPreference(
            likesCommercial: false,
            food: new List<string> { "French", "Japanese" },
            culture: new List<string> { "Theaters" },
            entertainment: new List<string> { "Concerts" },
            placeTypes: new List<string> { "City" });
        
        user.SetPreferences(preferences2);
        _userRepository.Update(user);
        await DbContext.SaveChangesAsync();

        // Act
        _repository.Update(user.Preferences!);
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.Food.Should().HaveCount(2);
        updatedUser.Preferences.LikesCommercial.Should().BeFalse();
    }
}
