using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Group.Command.UpdateGroupById;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Group.Command.UpdateGroupById;

[TestFixture]
public class UpdateGroupByIdHandlerTests : TestBase
{
    private UpdateGroupByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
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
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new UpdateGroupByIdHandler(_groupRepository, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithOwner_ShouldUpdateGroup()
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

        var newName = $"Updated {Guid.NewGuid().ToString()[..10]}";
        var newDate = DateTime.UtcNow.AddDays(60);
        var command = new UpdateGroupByIdCommand
        {
            UserId = user.Id,
            GroupId = group.Id,
            Name = newName,
            TripExpectedDate = newDate
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedGroup = await _groupRepository.GetByIdAsync(group.Id, CancellationToken.None);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Name.Should().Be(newName);
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

        var command = new UpdateGroupByIdCommand
        {
            UserId = member.Id,
            GroupId = group.Id,
            Name = $"Updated Group {Guid.NewGuid():N}",
            TripExpectedDate = DateTime.UtcNow.AddDays(60)
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
