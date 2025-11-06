using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Domain.ValueObjects.TripPreferences;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
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

        var command = new AdminSetUserPreferencesByUserIdCommand
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
        updatedUser.Preferences!.Food.Should().HaveCount(command.Food.Count);
        updatedUser.Preferences!.Culture.Should().HaveCount(command.Culture.Count);
        updatedUser.Preferences!.Entertainment.Should().HaveCount(command.Entertainment.Count);
        updatedUser.Preferences!.PlaceTypes.Should().HaveCount(command.PlaceTypes.Count);
    }
}
