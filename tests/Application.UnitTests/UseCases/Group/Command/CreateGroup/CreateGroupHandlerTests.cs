using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Group.Command.CreateGroup;

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
    public void Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new CreateGroupCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Name = $"Test Group {Guid.NewGuid():N}",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WithUserWithoutPreferences_ShouldThrowBadRequestException()
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

        var command = new CreateGroupCommand
        {
            UserId = user.Id,
            Name = $"Test Group {Guid.NewGuid():N}",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}
