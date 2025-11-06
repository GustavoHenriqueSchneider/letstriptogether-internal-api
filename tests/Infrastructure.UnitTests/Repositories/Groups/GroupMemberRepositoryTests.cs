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
public class GroupMemberRepositoryTests : TestBase
{
    private GroupMemberRepository _repository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new GroupMemberRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task GetAllByGroupIdAsync_WithMembers_ShouldReturnPaginatedResults()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Group(groupName, DateTime.UtcNow.AddDays(30));
        
        for (int i = 0; i < 5; i++)
        {
            var email = TestDataHelper.GenerateRandomEmail();
            var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
            var userName = TestDataHelper.GenerateRandomName();
            var user = new User(userName, email, passwordHash, role);
            await _userRepository.AddAsync(user, CancellationToken.None);
            group.AddMember(user, isOwner: i == 0);
        }
        
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var (data, hits) = await _repository.GetAllByGroupIdAsync(group.Id, 1, 10, CancellationToken.None);

        // Assert
        data.Should().HaveCount(5);
        hits.Should().Be(5);
    }

    [Test]
    public async Task GetAllByUserIdAsync_WithGroups_ShouldReturnMemberships()
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

        var group1 = new Group($"Group 1 {Guid.NewGuid().ToString()[..8]}", DateTime.UtcNow.AddDays(30));
        group1.AddMember(user, isOwner: true);
        
        var group2 = new Group($"Group 2 {Guid.NewGuid().ToString()[..8]}", DateTime.UtcNow.AddDays(30));
        group2.AddMember(user, isOwner: false);
        
        await _groupRepository.AddAsync(group1, CancellationToken.None);
        await _groupRepository.AddAsync(group2, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByUserIdAsync(user.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }
}
