using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.Tests.UseCases.User.Command.DeleteCurrentUser;

[TestFixture]
public class DeleteCurrentUserHandlerTests : TestBase
{
    private DeleteCurrentUserHandler _handler = null!;
    private IRedisService _redisService = null!;
    private IUnitOfWork _unitOfWork = null!;
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
        
        _handler = new DeleteCurrentUserHandler(_redisService, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidUserId_ShouldDeleteUser()
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

        var command = new DeleteCurrentUserCommand { UserId = user.Id };
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        _handler = new DeleteCurrentUserHandler(_redisService, _unitOfWork, _userRepository);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var userExists = await _userRepository.ExistsByIdAsync(user.Id, CancellationToken.None);
        userExists.Should().BeFalse();
        redisServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new DeleteCurrentUserCommand { UserId = TestDataHelper.GenerateRandomGuid() };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.NotFoundException>();
    }
}
