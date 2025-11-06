using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;

[TestFixture]
public class AdminDeleteUserByIdHandlerTests : TestBase
{
    private AdminDeleteUserByIdHandler _handler = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IRedisService _redisService = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        
        _handler = new AdminDeleteUserByIdHandler(_redisService, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidUser_ShouldDeleteUser()
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

        var command = new AdminDeleteUserByIdCommand
        {
            UserId = user.Id
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }
}
