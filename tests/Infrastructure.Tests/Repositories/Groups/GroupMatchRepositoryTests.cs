using FluentAssertions;
using Infrastructure.Tests.Common;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Destinations;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Infrastructure.Tests.Repositories.Groups;

[TestFixture]
public class GroupMatchRepositoryTests : TestBase
{
    private GroupMatchRepository _repository = null!;
    private GroupRepository _groupRepository = null!;
    private DestinationRepository _destinationRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new GroupMatchRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _destinationRepository = new DestinationRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task GetByGroupIdAsync_WithMatches_ShouldReturnPaginatedResults()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        for (int i = 0; i < 5; i++)
        {
            var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await _destinationRepository.AddAsync(destination, CancellationToken.None);
            await DbContext.SaveChangesAsync();
            
            var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
            typeof(GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
            typeof(GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
            await DbContext.Set<GroupMatch>().AddAsync(match, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var (data, hits) = await _repository.GetByGroupIdAsync(group.Id, 1, 10, CancellationToken.None);

        // Assert
        data.Should().HaveCount(5);
        hits.Should().Be(5);
    }

    [Test]
    public async Task GetByGroupAndDestinationAsync_WithMatch_ShouldReturnMatch()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _destinationRepository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch
        {
            GroupId = group.Id,
            DestinationId = destination.Id
        };
        typeof(GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
        typeof(GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
        await _repository.AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByGroupAndDestinationAsync(group.Id, destination.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.DestinationId.Should().Be(destination.Id);
    }

    [Test]
    public async Task GetAllMatchesByGroupAsync_WithMatches_ShouldReturnAllMatches()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        for (int i = 0; i < 3; i++)
        {
            var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await _destinationRepository.AddAsync(destination, CancellationToken.None);
            await DbContext.SaveChangesAsync();
            
            var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
            typeof(GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
            typeof(GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
            await DbContext.Set<GroupMatch>().AddAsync(match, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllMatchesByGroupAsync(group.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public async Task GetByIdWithRelationsAsync_WithMatch_ShouldReturnMatchWithGroup()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _destinationRepository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
        typeof(GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
        typeof(GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
        await _repository.AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithRelationsAsync(group.Id, match.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(match.Id);
        result.Group.Should().NotBeNull();
        result.Group.Id.Should().Be(group.Id);
    }

    [Test]
    public async Task GetByIdWithRelationsAsync_WithNonExistentMatch_ShouldReturnNull()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(LetsTripTogether.InternalApi.Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, LetsTripTogether.InternalApi.Domain.Security.Roles.User);
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var nonExistentMatchId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdWithRelationsAsync(group.Id, nonExistentMatchId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
