using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;

[TestFixture]
public class AdminAnonymizeUserByIdHandlerTests : TestBase
{
    private AdminAnonymizeUserByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private GroupMemberRepository _groupMemberRepository = null!;
    private IRedisService _redisService = null!;
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
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        
        _handler = new AdminAnonymizeUserByIdHandler(
            _groupMemberRepository,
            _redisService,
            _unitOfWork,
            _userGroupInvitationRepository,
            _userRepository,
            _userRoleRepository);
    }

    [Test]
    public async Task Handle_WithValidUser_ShouldAnonymizeUser()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        await _roleRepository.AddOrUpdateAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new AdminAnonymizeUserByIdCommand
        {
            UserId = user.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var anonymizedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        anonymizedUser.Should().NotBeNull();
        anonymizedUser!.Email.Should().Contain("anon_");
        anonymizedUser!.IsAnonymous.Should().BeTrue();
    }
}
