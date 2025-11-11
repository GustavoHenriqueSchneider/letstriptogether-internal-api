using Application.Common.Interfaces.Services;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Infrastructure.Tests.Common;
using NUnit.Framework;

namespace Infrastructure.Tests.Repositories.Users;

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
        var role = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, global::Domain.Security.Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { new TripPreference.Food(TripPreference.Food.Restaurant).ToString() },
            culture: new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
            entertainment: new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
            placeTypes: new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString() });
        
        user.SetPreferences(preferences);

        // Act
        await _repository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.LikesCommercial.Should().Be(preferences.LikesCommercial);
        updatedUser.Preferences!.Food.Should().BeEquivalentTo(preferences.Food);
        updatedUser.Preferences!.Culture.Should().BeEquivalentTo(preferences.Culture);
        updatedUser.Preferences!.Entertainment.Should().BeEquivalentTo(preferences.Entertainment);
        updatedUser.Preferences!.PlaceTypes.Should().BeEquivalentTo(preferences.PlaceTypes);
    }

    [Test]
    public async Task AddOrUpdateAsync_WithExistingPreference_ShouldUpdatePreference()
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

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences1 = new UserPreference(
            likesCommercial: true,
            food: new List<string> { new TripPreference.Food(TripPreference.Food.Restaurant).ToString() },
            culture: new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
            entertainment: new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Adventure).ToString() },
            placeTypes: new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString() });
        
        user.SetPreferences(preferences1);
        _userRepository.Update(user);
        await _repository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var preferences2 = new UserPreference(
            likesCommercial: false,
            food: new List<string> { new TripPreference.Food(TripPreference.Food.Restaurant).ToString() },
            culture: new List<string> { new TripPreference.Culture(TripPreference.Culture.Historical).ToString() },
            entertainment: new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Park).ToString() },
            placeTypes: new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Rural).ToString() });
        
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
        updatedUser.Preferences!.LikesCommercial.Should().Be(preferences2.LikesCommercial);
        updatedUser.Preferences!.Food.Should().BeEquivalentTo(preferences2.Food);
        updatedUser.Preferences!.Culture.Should().BeEquivalentTo(preferences2.Culture);
        updatedUser.Preferences!.Entertainment.Should().BeEquivalentTo(preferences2.Entertainment);
        updatedUser.Preferences!.PlaceTypes.Should().BeEquivalentTo(preferences2.PlaceTypes);
    }
}
