using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.GroupDestinationVote.Command.UpdateDestinationVoteById;
using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;

[TestFixture]
public class UpdateDestinationVoteByIdHandlerTests : TestBase
{
    private UpdateDestinationVoteByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMatchRepository _groupMatchRepository = null!;
    private GroupMemberDestinationVoteRepository _groupMemberDestinationVoteRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;
    private Mock<INotificationService> _notificationServiceMock = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupMatchRepository = new GroupMatchRepository(DbContext);
        _groupMemberDestinationVoteRepository = new GroupMemberDestinationVoteRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        _notificationServiceMock = new Mock<INotificationService>();
        
        _handler = new UpdateDestinationVoteByIdHandler(
            _groupMatchRepository,
            _groupMemberDestinationVoteRepository,
            _groupRepository,
            _unitOfWork,
            _userRepository,
            _notificationServiceMock.Object);
    }

    [Test]
    public async Task Handle_WithValidVote_ShouldUpdateVote()
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

        var groupMember = group.Members.First();
        var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, false);
        await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            DestinationVoteId = vote.Id,
            IsApproved = true
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedVote = await _groupMemberDestinationVoteRepository.GetByIdAsync(vote.Id, CancellationToken.None);
        updatedVote.Should().NotBeNull();
        updatedVote!.IsApproved.Should().BeTrue();
    }

    [Test]
    public async Task Handle_WithNonOwner_ShouldThrowBadRequestException()
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

        var ownerEmail = TestDataHelper.GenerateRandomEmail();
        var ownerPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var ownerName = TestDataHelper.GenerateRandomName();
        var owner = new Domain.Aggregates.UserAggregate.Entities.User(ownerName, ownerEmail, ownerPasswordHash, role);
        await _userRepository.AddAsync(owner, CancellationToken.None);
        
        var memberEmail = TestDataHelper.GenerateRandomEmail();
        var memberPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var memberName = TestDataHelper.GenerateRandomName();
        var member = new Domain.Aggregates.UserAggregate.Entities.User(memberName, memberEmail, memberPasswordHash, role);
        await _userRepository.AddAsync(member, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName = TestDataHelper.GenerateRandomGroupName();
        var group = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName, DateTime.UtcNow.AddDays(30));
        group.AddMember(owner, isOwner: true);
        group.AddMember(member, isOwner: false);
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var destination = new Domain.Aggregates.DestinationAggregate.Entities.Destination
        {
            Address = "Test Address",
            Description = "Test Description"
        };
        await DbContext.Set<Domain.Aggregates.DestinationAggregate.Entities.Destination>().AddAsync(destination, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var ownerMember = group.Members.First(m => m.UserId == owner.Id);
        var vote = new GroupMemberDestinationVote(ownerMember.Id, destination.Id, false);
        await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = member.Id,
            GroupId = group.Id,
            DestinationVoteId = vote.Id,
            IsApproved = true
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Test]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            GroupId = TestDataHelper.GenerateRandomGuid(),
            DestinationVoteId = TestDataHelper.GenerateRandomGuid(),
            IsApproved = true
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found.");
    }

    [Test]
    public async Task Handle_WithUserNotMemberOfGroup_ShouldThrowBadRequestException()
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
        await _groupRepository.AddAsync(group, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            DestinationVoteId = TestDataHelper.GenerateRandomGuid(),
            IsApproved = true
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("You are not a member of this group.");
    }

    [Test]
    public async Task Handle_WithNonExistentVote_ShouldThrowNotFoundException()
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

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            DestinationVoteId = TestDataHelper.GenerateRandomGuid(),
            IsApproved = true
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Vote not found.");
    }

    [Test]
    public async Task Handle_WithExistingMatch_ShouldThrowBadRequestException()
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

        var groupMember = group.Members.First();
        var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, false);
        await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var match = new Domain.Aggregates.GroupAggregate.Entities.GroupMatch
        {
            GroupId = group.Id,
            DestinationId = destination.Id
        };
        await _groupMatchRepository.AddAsync(match, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            DestinationVoteId = vote.Id,
            IsApproved = true
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("There is already a match with this vote, you can not change it.");
    }

    [Test]
    public async Task Handle_WithIsApprovedFalse_ShouldNotCreateMatch()
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

        var groupMember = group.Members.First();
        var vote = new GroupMemberDestinationVote(groupMember.Id, destination.Id, true);
        await _groupMemberDestinationVoteRepository.AddAsync(vote, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new UpdateDestinationVoteByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            DestinationVoteId = vote.Id,
            IsApproved = false
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedVote = await _groupMemberDestinationVoteRepository.GetByIdAsync(vote.Id, CancellationToken.None);
        updatedVote.Should().NotBeNull();
        updatedVote!.IsApproved.Should().BeFalse();
        
        var matches = await _groupMatchRepository.GetByGroupAndDestinationAsync(group.Id, destination.Id, CancellationToken.None);
        matches.Should().BeNull();
    }
}
