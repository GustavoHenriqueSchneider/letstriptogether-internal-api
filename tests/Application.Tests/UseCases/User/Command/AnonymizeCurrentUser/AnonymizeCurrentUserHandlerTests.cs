using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Tests.Common;
using Application.UseCases.User.Command.AnonymizeCurrentUser;
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

namespace Application.Tests.UseCases.User.Command.AnonymizeCurrentUser;

[TestFixture]
public class AnonymizeCurrentUserHandlerTests : TestBase
{
    private AnonymizeCurrentUserHandler _handler = null!;
    private IRedisService _redisService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private UserGroupInvitationRepository _userGroupInvitationRepository = null!;
    private UserRepository _userRepository = null!;
    private UserRoleRepository _userRoleRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _groupMemberRepository = new GroupMemberRepository(DbContext);
        _userGroupInvitationRepository = new UserGroupInvitationRepository(DbContext);
        _userRepository = new UserRepository(DbContext);
        _userRoleRepository = new UserRoleRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        
        _handler = new AnonymizeCurrentUserHandler(
            _groupMemberRepository,
            _redisService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository,
            _userRoleRepository);
    }

    [Test]
    public async Task Handle_WithValidUserId_ShouldAnonymizeUser()
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

        var command = new AnonymizeCurrentUserCommand { UserId = user.Id };
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        _handler = new AnonymizeCurrentUserHandler(
            _groupMemberRepository,
            _redisService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository,
            _userRoleRepository);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Contain("anon_");
        updatedUser!.Email.Should().Contain("@deleted.local");
        redisServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new AnonymizeCurrentUserCommand { UserId = TestDataHelper.GenerateRandomGuid() };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
