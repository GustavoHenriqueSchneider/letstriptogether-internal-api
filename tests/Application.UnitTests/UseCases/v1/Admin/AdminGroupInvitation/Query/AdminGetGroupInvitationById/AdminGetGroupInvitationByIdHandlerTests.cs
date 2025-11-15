using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;

[TestFixture]
public class AdminGetGroupInvitationByIdHandlerTests : TestBase
{
    private AdminGetGroupInvitationByIdHandler _handler = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new AdminGetGroupInvitationByIdHandler(_groupInvitationRepository, _groupRepository);
    }

    [Test]
    public async Task Handle_WithValidInvitation_ShouldReturnInvitation()
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

        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = group.Id,
            InvitationId = invitation.Id
        };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().NotBeNull();
        result.ExpirationDate.Should().BeCloseTo(invitation.ExpirationDate, TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task Handle_WithNonExistentGroup_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = TestDataHelper.GenerateRandomGuid(),
            InvitationId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Group not found.");
    }

    [Test]
    public async Task Handle_WithNonExistentInvitation_ShouldThrowNotFoundException()
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

        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = group.Id,
            InvitationId = TestDataHelper.GenerateRandomGuid()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Group invitation not found.");
    }

    [Test]
    public async Task Handle_WithInvitationFromDifferentGroup_ShouldThrowBadRequestException()
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

        var groupName1 = TestDataHelper.GenerateRandomGroupName();
        var group1 = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName1, DateTime.UtcNow.AddDays(30));
        group1.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group1, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var groupName2 = TestDataHelper.GenerateRandomGroupName();
        var group2 = new Domain.Aggregates.GroupAggregate.Entities.Group(groupName2, DateTime.UtcNow.AddDays(30));
        group2.AddMember(user, isOwner: true);
        await _groupRepository.AddAsync(group2, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var invitation = new Domain.Aggregates.GroupAggregate.Entities.GroupInvitation(group2.Id);
        await _groupInvitationRepository.AddAsync(invitation, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var query = new AdminGetGroupInvitationByIdQuery
        {
            GroupId = group1.Id,
            InvitationId = invitation.Id
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("The invitation does not belong to this group.");
    }
}
