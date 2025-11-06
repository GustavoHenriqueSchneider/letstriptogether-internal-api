using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Query.GetGroupMatchById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupMatch.Query.GetGroupMatchById;

[TestFixture]
public class GetGroupMatchByIdHandlerTests : TestBase
{
    private GetGroupMatchByIdHandler _handler = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new GetGroupMatchByIdHandler(_groupRepository, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidMatch_ShouldReturnMatch()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var destination = new LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await DbContext.Set<LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities.Destination>().AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
        typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
        typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
        await DbContext.Set<LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch>().AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new GetGroupMatchByIdQuery
        {
            UserId = user.Id,
            GroupId = group.Id,
            MatchId = match.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.DestinationId.Should().Be(destination.Id);
    }
}
