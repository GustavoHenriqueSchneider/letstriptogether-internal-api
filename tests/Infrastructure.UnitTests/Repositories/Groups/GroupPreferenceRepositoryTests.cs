using Application.Common.Interfaces.Services;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories.Groups;

[TestFixture]
public class GroupPreferenceRepositoryTests : TestBase
{
    private GroupPreferenceRepository _repository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;
    private UserPreferenceRepository _userPreferenceRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
    }

    [Test]
    public async Task GetByGroupIdAsync_WithExistingGroup_ShouldReturnPreferences()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        
        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { new TripPreference.Food(TripPreference.Food.Restaurant).ToString() },
            culture: new List<string> { new TripPreference.Culture(TripPreference.Culture.Museum).ToString() },
            entertainment: new List<string> { new TripPreference.Entertainment(TripPreference.Entertainment.Attraction).ToString() },
            placeTypes: new List<string> { new TripPreference.PlaceType(TripPreference.PlaceType.Beach).ToString() });
        
        user.SetPreferences(preferences);
        await _userPreferenceRepository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupPreferences = group.UpdatePreferences(preferences);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await _repository.AddAsync(groupPreferences, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByGroupIdAsync(group.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.LikesCommercial.Should().BeTrue();
    }

    [Test]
    public async Task GetByGroupIdAsync_WithNonExistingGroup_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByGroupIdAsync(TestDataHelper.GenerateRandomGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
