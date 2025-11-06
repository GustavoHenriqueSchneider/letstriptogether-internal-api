using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

[TestFixture]
public class GetAllGroupMatchesByIdHandlerTests : TestBase
{
    private GetAllGroupMatchesByIdHandler _handler = null!;
    private GroupMatchRepository _groupMatchRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupMatchRepository = new GroupMatchRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new GetAllGroupMatchesByIdHandler(_groupMatchRepository, _groupRepository, _userRepository);
    }

    [Test]
    public async Task Handle_WithMatches_ShouldReturnPaginatedResults()
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

        for (int i = 0; i < 3; i++)
        {
            var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
            {
                Address = $"Address {i}",
                Description = $"Description {i}"
            };
            await DbContext.Set<LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination>().AddAsync(destination, CancellationToken.None);
            await DbContext.SaveChangesAsync();
            
            var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch
            {
                GroupId = group.Id,
                DestinationId = destination.Id
            };
            typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
            typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
            await _groupMatchRepository.AddAsync(match, CancellationToken.None);
        }
        await DbContext.SaveChangesAsync();

        var query = new GetAllGroupMatchesByIdQuery
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
        result.Data.Should().HaveCount(3);
        result.Hits.Should().Be(3);
    }

    [Test]
    public async Task Handle_WithNonMember_ShouldThrowBadRequestException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var nonMemberEmail = TestDataHelper.GenerateRandomEmail();
        var nonMemberPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var nonMemberName = TestDataHelper.GenerateRandomName();
        var nonMember = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(nonMemberName, nonMemberEmail, nonMemberPasswordHash, role);
        await _userRepository.AddAsync(nonMember, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = $"Test Group {Guid.NewGuid():N}";
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetAllGroupMatchesByIdQuery
        {
            UserId = nonMember.Id,
            GroupId = group.Id,
            PageNumber = 1,
            PageSize = 10
        };

        // Act & Assert
        Assert.ThrowsAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>(async () =>
            await _handler.Handle(query, CancellationToken.None));
    }
}
