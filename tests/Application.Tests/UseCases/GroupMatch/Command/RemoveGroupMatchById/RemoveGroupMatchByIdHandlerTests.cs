using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.GroupMatch.Command.RemoveGroupMatchById;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using NUnit.Framework;

namespace Application.Tests.UseCases.GroupMatch.Command.RemoveGroupMatchById;

[TestFixture]
public class RemoveGroupMatchByIdHandlerTests : TestBase
{
    private RemoveGroupMatchByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository = null!;
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
        _unitOfWork = DbContext;
        _groupMemberDestinationVoteRepository = new GroupMemberDestinationVoteRepository(DbContext);
        _groupMatchRepository = new GroupMatchRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new RemoveGroupMatchByIdHandler(
            _groupMemberDestinationVoteRepository,
            _groupMatchRepository,
            _groupRepository,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithValidMatch_ShouldRemoveMatch()
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

        var groupMember = group.Members.First();
        var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, true);
        await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        
        var match = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch();
        typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("GroupId")!.SetValue(match, group.Id);
        typeof(LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch).GetProperty("DestinationId")!.SetValue(match, destination.Id);
        await DbContext.Set<LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.GroupMatch>().AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            MatchId = match.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var removedMatch = await _groupMatchRepository.GetByIdAsync(match.Id, CancellationToken.None);
        removedMatch.Should().BeNull();
        
        var updatedVote = await _groupMemberDestinationVoteRepository.GetByIdAsync(vote.Id, CancellationToken.None);
        updatedVote.Should().NotBeNull();
        updatedVote!.IsApproved.Should().BeFalse();
    }

    [Test]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            GroupId = TestDataHelper.GenerateRandomGuid(),
            MatchId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithNonExistentGroup_ShouldThrowNotFoundException()
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
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = user.Id,
            GroupId = TestDataHelper.GenerateRandomGuid(),
            MatchId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("Group not found.");
    }

    [Test]
    public async Task Handle_WithUserNotMemberOfGroup_ShouldThrowBadRequestException()
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
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            MatchId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>()
            .WithMessage("You are not a member of this group.");
    }

    [Test]
    public async Task Handle_WithNonExistentMatch_ShouldThrowNotFoundException()
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
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            MatchId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("The match was not found for this group.");
    }

    [Test]
    public async Task Handle_WithNonExistentVote_ShouldThrowNotFoundException()
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

        var command = new RemoveGroupMatchByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            MatchId = match.Id
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>()
            .WithMessage("The vote was not found.");
    }
}
