using Application.Common.Interfaces.Services;
using Domain.Aggregates.DestinationAggregate.Entities;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using FluentAssertions;
using Infrastructure.Repositories.Destinations;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Repositories.Groups;

[TestFixture]
public class GroupMemberDestinationVoteRepositoryTests : TestBase
{
    private GroupMemberDestinationVoteRepository _repository = null!;
    private GroupRepository _groupRepository = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private DestinationRepository _destinationRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _repository = new GroupMemberDestinationVoteRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _destinationRepository = new DestinationRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
    }

    [Test]
    public async Task ExistsByGroupMemberDestinationVoteByIdsAsync_WithExistingVote_ShouldReturnTrue()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, global::Domain.Security.Roles.User);
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

        var destination = new Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await _destinationRepository.AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupMember = group.Members.First();
        var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, true);
        await _repository.AddAsync(vote, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByGroupMemberDestinationVoteByIdsAsync(
            groupMember.Id, destination.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task GetByMemberIdAsync_WithVotes_ShouldReturnPaginatedResults()
    {
        // Arrange
        var role = await _roleRepository.GetByNameAsync(global::Domain.Security.Roles.User, CancellationToken.None);

        if (role is null)
        {
            role = new Role();
            typeof(Role).GetProperty("Name")!.SetValue(role, global::Domain.Security.Roles.User);
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

        var groupMember = group.Members.First();
        
        for (int i = 0; i < 5; i++)
        {
            var destination = new Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await _destinationRepository.AddAsync(destination, CancellationToken.None);
            
            var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, i % 2 == 0);
            await _repository.AddAsync(vote, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        // Act
        var (data, hits) = await _repository.GetByMemberIdAsync(groupMember.Id, 1, 10, CancellationToken.None);

        // Assert
        data.Should().HaveCount(5);
        hits.Should().Be(5);
    }
}
