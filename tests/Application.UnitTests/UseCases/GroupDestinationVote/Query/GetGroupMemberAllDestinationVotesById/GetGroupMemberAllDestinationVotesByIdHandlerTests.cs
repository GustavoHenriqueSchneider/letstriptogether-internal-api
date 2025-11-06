using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

[TestFixture]
public class GetGroupMemberAllDestinationVotesByIdHandlerTests : TestBase
{
    private GetGroupMemberAllDestinationVotesByIdHandler _handler = null!;
    private GroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupMemberDestinationVoteRepository = new GroupMemberDestinationVoteRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new GetGroupMemberAllDestinationVotesByIdHandler(
            _groupMemberDestinationVoteRepository,
            _groupRepository,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithVotes_ShouldReturnPaginatedResults()
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

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupMember = group.Members.First();
        
        for (int i = 0; i < 5; i++)
        {
            var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await DbContext.Set<LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination>().AddAsync(destination, CancellationToken.None);
            await DbContext.SaveChangesAsync();
            
            var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, i % 2 == 0);
            await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        var query = new GetGroupMemberAllDestinationVotesByIdQuery
        {
            UserId = user.Id,
            GroupId = group.Id,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(5);
        result.Hits.Should().Be(5);
    }
}
