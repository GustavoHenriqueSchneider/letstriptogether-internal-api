using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.SetCurrentUserPreferences;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Command.SetCurrentUserPreferences;

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
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new SetCurrentUserPreferencesCommand
        {
            UserId = user.Id,
            LikesCommercial = true,
            Food = new List<string> { "Italian", "Brazilian" },
            Culture = new List<string> { "Museums" },
            Entertainment = new List<string> { "Nightlife" },
            PlaceTypes = new List<string> { "Beach", "Mountain" }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdWithPreferencesAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Preferences.Should().NotBeNull();
        updatedUser.Preferences!.Food.Should().HaveCount(2);
    }

    [Test]
    public async Task Handle_WithUserInGroup_ShouldUpdateGroupPreferences()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
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

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
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
            Food = new List<string> { "French", "Japanese" },
            Culture = new List<string> { "Theaters" },
            Entertainment = new List<string> { "Concerts" },
            PlaceTypes = new List<string> { "City" }
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetGroupWithPreferencesAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Preferences.Should().NotBeNull();
    }

    [Test]
    public void Handle_WithInvalidUser_ShouldThrowNotFoundException()
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
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}
