using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Tests.Common;
using Application.UseCases.User.Command.SetCurrentUserPreferences;
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

namespace Application.Tests.UseCases.User.Command.SetCurrentUserPreferences;

[TestFixture]
public class SetCurrentUserPreferencesHandlerTests : TestBase
{
    private SetCurrentUserPreferencesHandler _handler = null!;
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
        
        _handler = new SetCurrentUserPreferencesHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _unitOfWork,
            _userPreferenceRepository,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidPreferences_ShouldUpdateUserPreferences()
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

        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = user.Id,
            LikesCommercial = true,
            Food = new List<string> { new TripPreference(TripPreference.Food.Restaurant) },
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
        updatedUser.Preferences!.LikesCommercial.Should().Be(command.LikesCommercial);
        updatedUser.Preferences!.Food.Should().BeEquivalentTo(command.Food);
        updatedUser.Preferences!.Culture.Should().BeEquivalentTo(command.Culture);
        updatedUser.Preferences!.Entertainment.Should().BeEquivalentTo(command.Entertainment);
        updatedUser.Preferences!.PlaceTypes.Should().BeEquivalentTo(command.PlaceTypes);
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

        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { new TripPreference(TripPreference.Food.Restaurant) },
            culture: new List<string> { new TripPreference(TripPreference.Culture.Museum) },
            entertainment: new List<string> { new TripPreference(TripPreference.Entertainment.Attraction) },
            placeTypes: new List<string> { new TripPreference(TripPreference.PlaceType.Beach) });
        
        user.SetPreferences(preferences);
        await _userPreferenceRepository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        group.UpdatePreferences(preferences);
        
        await _groupRepository.AddAsync(group, CancellationToken.None);
        var groupPrefs = group.Preferences;
        await _groupPreferenceRepository.AddAsync(groupPrefs, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = user.Id,
            LikesCommercial = false,
            Food = new List<string> { new TripPreference(TripPreference.Food.Restaurant) },
            Culture = new List<string> { new TripPreference(TripPreference.Culture.Education) },
            Entertainment = new List<string> { new TripPreference(TripPreference.Entertainment.Tour) },
            PlaceTypes = new List<string> { new TripPreference(TripPreference.PlaceType.Trail) }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetGroupWithPreferencesAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Preferences.Should().NotBeNull();
    }

    [Test]
    public async Task Handle_WithInvalidUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            LikesCommercial = true,
            Food = new List<string>(),
            Culture = new List<string>(),
            Entertainment = new List<string>(),
            PlaceTypes = new List<string>()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
