using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.User.Query.GetCurrentUser;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Security;
using Domain.ValueObjects.TripPreferences;
using FluentAssertions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.User.Query.GetCurrentUser;

[TestFixture]
public class GetCurrentUserHandlerTests : TestBase
{
    private GetCurrentUserHandler _handler = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;
    private UserPreferenceRepository _userPreferenceRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        _handler = new GetCurrentUserHandler(_userRepository);
    }

    [Test]
    public async Task Handle_WithValidUserId_ShouldReturnUser()
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

        var query = new GetCurrentUserQuery { UserId = user.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.Name.Should().Be(userName);
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetCurrentUserQuery { UserId = TestDataHelper.GenerateRandomGuid() };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task Handle_WithUserWithPreferences_ShouldReturnPreferences()
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
            entertainment: new List<string> { new TripPreference(TripPreference.Entertainment.Attraction), },
            placeTypes: new List<string> { new TripPreference(TripPreference.PlaceType.Beach) });
        
        user.SetPreferences(preferences);
        await _userPreferenceRepository.AddAsync(user.Preferences!, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetCurrentUserQuery { UserId = user.Id };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Preferences.Should().NotBeNull();
        result.Preferences!.LikesCommercial.Should().BeTrue();
    }
}
