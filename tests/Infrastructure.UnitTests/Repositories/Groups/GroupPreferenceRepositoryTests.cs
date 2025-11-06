using FluentAssertions;
using Infrastructure.UnitTests.Common;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
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

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new GroupPreferenceRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task GetByGroupIdAsync_WithExistingGroup_ShouldReturnPreferences()
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

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        
        var preferences = new UserPreference(
            likesCommercial: true,
            food: new List<string> { "Italian" },
            culture: new List<string> { "Museums" },
            entertainment: new List<string> { "Nightlife" },
            placeTypes: new List<string> { "Beach" });
        
        user.SetPreferences(preferences);
        _userRepository.Update(user);
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
