using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Tests.Common;
using Application.UseCases.Group.Command.CreateGroup;
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

namespace Application.Tests.UseCases.Group.Command.CreateGroup;

[TestFixture]
public class CreateGroupHandlerTests : TestBase
{
    private CreateGroupHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private GroupPreferenceRepository _groupPreferenceRepository = null!;
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
        _unitOfWork = DbContext;
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _groupPreferenceRepository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _userPreferenceRepository = new UserPreferenceRepository(DbContext);
        
        _handler = new CreateGroupHandler(
            _groupMemberRepository,
            _groupPreferenceRepository,
            _groupRepository,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidData_ShouldCreateGroup()
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
        var command = new CreateGroupCommand
        {
            UserId = user.Id,
            Name = groupName,
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var group = await _groupRepository.GetByIdAsync(result.Id, CancellationToken.None);
        group.Should().NotBeNull();
        group!.Name.Should().Be(groupName);
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateGroupCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Name = TestDataHelper.GenerateRandomGroupName(),
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task Handle_WithUserWithoutPreferences_ShouldThrowBadRequestException()
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

        var command = new CreateGroupCommand
        {
            UserId = user.Id,
            Name = TestDataHelper.GenerateRandomGroupName(),
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
