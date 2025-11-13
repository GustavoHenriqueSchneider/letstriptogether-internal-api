using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;
using Domain.Security;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;

[TestFixture]
public class AdminSetUserPreferencesByUserIdHandlerTests : TestBase
{
    private AdminSetUserPreferencesByUserIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupPreferenceRepository _groupPreferenceRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserPreferenceRepository _userPreferenceRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupPreferenceRepository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new AdminSetUserPreferencesByUserIdHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _unitOfWork,
            _userPreferenceRepository,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidPreferences_ShouldUpdatePreferences()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = user.Id,
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            Entertainment = new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Beach), "mountain" }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.LikesShopping.Should().Be(command.LikesShopping);
        updatedUser.Preferences!.LikesGastronomy.Should().Be(command.LikesGastronomy);
        updatedUser.Preferences!.Culture.Should().BeEquivalentTo(command.Culture);
        updatedUser.Preferences!.Entertainment.Should().BeEquivalentTo(command.Entertainment);
        updatedUser.Preferences!.PlaceTypes.Should().BeEquivalentTo(command.PlaceTypes);
    }

    [Test]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = Guid.NewGuid(),
            LikesShopping = true,
            LikesGastronomy = true,
            Culture = new List<string>(),
            Entertainment = new List<string>(),
            PlaceTypes = new List<string>()
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithUserInGroup_ShouldUpdateGroupPreferences()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create initial preferences
        var initialPreferences = new UserPreference(
            likesShopping: true,
            likesGastronomy: true,
            culture: new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            entertainment: new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            placeTypes: new List<string> { new TripPreference(TripPreference.PlaceType.Beach) });
        
        user.SetPreferences(initialPreferences);
        await _userPreferenceRepository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create group with user as member
        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        group.UpdatePreferences(initialPreferences);
        
        await _groupRepository.AddAsync(group, CancellationToken.None);
        var groupPrefs = group.Preferences;
        await _groupPreferenceRepository.AddAsync(groupPrefs, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = user.Id,
            LikesShopping = false,
            LikesGastronomy = true,
            Culture = new List<string> { new TripPreference(TripPreference.Culture.Education) },
            Entertainment = new List<string> { new TripPreference(TripPreference.Entertainment.Tour) },
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Trail) }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.LikesShopping.Should().Be(command.LikesShopping);

        var updatedGroup = await _groupRepository.GetGroupWithPreferencesAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Preferences.Should().NotBeNull();
    }

    [Test]
    public async Task Handle_WithUserInMultipleGroups_ShouldUpdateAllGroupPreferences()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
            await _roleRepository.AddAsync(role, CancellationToken.None);
            await DbContext.SaveChangesAsync();
        }

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create initial preferences
        var initialPreferences = new UserPreference(
            likesShopping: true,
            likesGastronomy: true,
            culture: new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            entertainment: new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            placeTypes: new List<string> { new TripPreference(TripPreference.PlaceType.Beach) });
        
        user.SetPreferences(initialPreferences);
        await _userPreferenceRepository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create first group
        var group1Name = TestDataHelper.GenerateRandomGroupName();
        var group1 = new Domain.Aggregates.GroupAggregate.Entities.Group(group1Name, DateTime.UtcNow.AddDays(30));
        group1.AddMember(user, isOwner: true);
        group1.UpdatePreferences(initialPreferences);
        await _groupRepository.AddAsync(group1, CancellationToken.None);
        await _groupPreferenceRepository.AddAsync(group1.Preferences, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Create second group
        var group2Name = TestDataHelper.GenerateRandomGroupName();
        var group2 = new Domain.Aggregates.GroupAggregate.Entities.Group(group2Name, DateTime.UtcNow.AddDays(30));
        group2.AddMember(user, isOwner: false);
        group2.UpdatePreferences(initialPreferences);
        await _groupRepository.AddAsync(group2, CancellationToken.None);
        await _groupPreferenceRepository.AddAsync(group2.Preferences, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            UserId = user.Id,
            LikesShopping = false,
            LikesGastronomy = true,
            Culture = new List<string> { new TripPreference(TripPreference.Culture.Education) },
            Entertainment = new List<string> { new TripPreference(TripPreference.Entertainment.Tour) },
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Trail) }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup1 = await _groupRepository.GetGroupWithPreferencesAsync(group1.Id, CancellationToken.None);
        updatedGroup1.Should().NotBeNull();
        updatedGroup1!.Preferences.Should().NotBeNull();

        var updatedGroup2 = await _groupRepository.GetGroupWithPreferencesAsync(group2.Id, CancellationToken.None);
        updatedGroup2.Should().NotBeNull();
        updatedGroup2!.Preferences.Should().NotBeNull();
    }
}
