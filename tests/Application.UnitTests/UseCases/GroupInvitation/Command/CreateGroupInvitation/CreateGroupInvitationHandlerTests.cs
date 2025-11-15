using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.UnitTests.Common;
using Application.UseCases.v1.GroupInvitation.Command.CreateGroupInvitation;
using Domain.Aggregates.GroupAggregate.Enums;
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

namespace Application.UnitTests.UseCases.GroupInvitation.Command.CreateGroupInvitation;

[TestFixture]
public class CreateGroupInvitationHandlerTests : TestBase
{
    private CreateGroupInvitationHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupInvitationRepository _groupInvitationRepository = null!;
    private GroupRepository _groupRepository = null!;
    private ITokenService _tokenService = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupInvitationRepository = new GroupInvitationRepository(DbContext);
        _groupRepository = new GroupRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var tokenServiceMock = new Mock<ITokenService>();
        tokenServiceMock.Setup(x => x.GenerateInvitationToken(It.IsAny<Guid>()))
            .Returns("test-token");
        _tokenService = tokenServiceMock.Object;
        
        _handler = new CreateGroupInvitationHandler(
            _groupInvitationRepository,
            _groupRepository,
            _tokenService,
            _unitOfWork,
            _userRepository);
    }

    [Test]
    public async Task Handle_WithOwner_ShouldCreateInvitation()
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

        var command = new CreateGroupInvitationCommand
        {
            UserId = user.Id,
            GroupId = group.Id
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-token");
        
        var invitation = await _groupInvitationRepository.GetByGroupAndStatusAsync(
            group.Id, GroupInvitationStatus.Active, CancellationToken.None);
        invitation.Should().NotBeNull();
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

        var command = new CreateGroupInvitationCommand
        {
            UserId = member.Id,
            GroupId = group.Id
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
