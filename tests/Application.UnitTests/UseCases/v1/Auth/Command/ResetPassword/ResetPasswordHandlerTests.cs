using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Application.UnitTests.Common;
using Application.UseCases.v1.Auth.Command.ResetPassword;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.v1.Auth.Command.ResetPassword;

[TestFixture]
public class ResetPasswordHandlerTests : TestBase
{
    private ResetPasswordHandler _handler = null!;
    private IPasswordHashService _passwordHashService = null!;
    private IRedisService _redisService = null!;
    private IUnitOfWork _unitOfWork = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private ITokenService _tokenService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        _unitOfWork = DbContext;
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var jwtSettings = new JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440,
            InvitationTokenValidityInMinutes = 10,
            ResetPasswordTokenValidityInMinutes = 10
        };
        
        var loggerMock = new Mock<ILogger<ITokenService>>();
        _tokenService = new TokenService(Microsoft.Extensions.Options.Options.Create(jwtSettings), new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler(), loggerMock.Object);
        
        _handler = new ResetPasswordHandler(_passwordHashService, _redisService, _unitOfWork, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidToken_ShouldResetPassword()
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
        var oldPasswordHash = _passwordHashService.HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, oldPasswordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var resetToken = _tokenService.GenerateResetPasswordToken(user.Id);
        var key = KeyHelper.UserResetPassword(user.Id);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync(resetToken);
        redisServiceMock.Setup(x => x.DeleteAsync(key)).Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        _handler = new ResetPasswordHandler(_passwordHashService, _redisService, _unitOfWork, _userRepository);

        var newPassword = TestDataHelper.GenerateValidPassword();
        var command = new ResetPasswordCommand
        {
            UserId = user.Id,
            Password = newPassword,
            BearerToken = resetToken
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        _passwordHashService.VerifyPassword(newPassword, updatedUser!.PasswordHash).Should().BeTrue();
        redisServiceMock.Verify(x => x.DeleteAsync(key), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidUserId_ShouldThrowNotFoundException()
    {
        // Arrange
        var redisServiceMock = new Mock<IRedisService>();
        _redisService = redisServiceMock.Object;
        _handler = new ResetPasswordHandler(_passwordHashService, _redisService, _unitOfWork, _userRepository);

        var command = new ResetPasswordCommand
        {
            UserId = TestDataHelper.GenerateRandomGuid(),
            Password = TestDataHelper.GenerateValidPassword(),
            BearerToken = "token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task Handle_WithInvalidToken_ShouldThrowUnauthorizedException()
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

        var key = KeyHelper.UserResetPassword(user.Id);
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync((string?)null);
        _redisService = redisServiceMock.Object;
        _handler = new ResetPasswordHandler(_passwordHashService, _redisService, _unitOfWork, _userRepository);

        var command = new ResetPasswordCommand
        {
            UserId = user.Id,
            Password = TestDataHelper.GenerateValidPassword(),
            BearerToken = "invalid-token"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
