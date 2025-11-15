using System.IdentityModel.Tokens.Jwt;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Application.UnitTests.Common;
using Application.UseCases.v1.Auth.Command.RefreshToken;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.RefreshToken;

[TestFixture]
public class RefreshTokenHandlerTests : TestBase
{
    private RefreshTokenHandler _handler = null!;
    private IRedisService _redisService = null!;
    private ITokenService _tokenService = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private IPasswordHashService _passwordHashService = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        
        var jwtSettings = new JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440
        };
        
        var loggerMock = new Mock<ILogger<ITokenService>>();
        _tokenService = new TokenService(Options.Create(jwtSettings), new JwtSecurityTokenHandler(), loggerMock.Object);
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new RefreshTokenHandler(_redisService, _tokenService, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidRefreshToken_ShouldReturnNewTokens()
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
        var password = TestDataHelper.GenerateValidPassword();
        var passwordHash = _passwordHashService.HashPassword(password);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var (accessToken, refreshToken) = _tokenService.GenerateTokens(user, DateTime.UtcNow.AddHours(24));
        var key = KeyHelper.UserRefreshToken(user.Id);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync(refreshToken);
        redisServiceMock.Setup(x => x.SetAsync(key, It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        
        _handler = new RefreshTokenHandler(_redisService, _tokenService, _userRepository);

        var command = new RefreshTokenCommand { RefreshToken = refreshToken };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Handle_WithInvalidRefreshToken_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        _redisService = redisServiceMock.Object;
        
        _handler = new RefreshTokenHandler(_redisService, _tokenService, _userRepository);

        var command = new RefreshTokenCommand { RefreshToken = "invalid-token" };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Test]
    public async Task Handle_WithMismatchedStoredToken_ShouldThrowUnauthorizedException()
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
        var password = TestDataHelper.GenerateValidPassword();
        var passwordHash = _passwordHashService.HashPassword(password);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var (_, refreshToken) = _tokenService.GenerateTokens(user, DateTime.UtcNow.AddHours(24));
        var key = KeyHelper.UserRefreshToken(user.Id);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync("different-token");
        _redisService = redisServiceMock.Object;
        
        _handler = new RefreshTokenHandler(_redisService, _tokenService, _userRepository);

        var command = new RefreshTokenCommand { RefreshToken = refreshToken };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}
