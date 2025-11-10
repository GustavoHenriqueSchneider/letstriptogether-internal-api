using Application.Common.Interfaces.Services;
using Application.Tests.Common;
using Application.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.Tests.UseCases.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;

[TestFixture]
public class AdminGetGroupMatchByIdHandlerTests : TestBase
{
    private AdminGetGroupMatchByIdHandler _handler = null!;
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
        
        _handler = new AdminGetGroupMatchByIdHandler(_groupRepository);
    }

    [Test]
    public async Task Handle_WithValidMatch_ShouldReturnMatch()
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

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var destination = new Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await DbContext.Set<Domain.Aggregates.DestinationAggregate.Entities.Destination>().AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var match = new Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
        typeof(Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
        typeof(Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
        await DbContext.Set<Domain.Aggregates.GroupAggregate.Entities.GroupMatch>().AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new AdminGetGroupMatchByIdQuery
        {
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
