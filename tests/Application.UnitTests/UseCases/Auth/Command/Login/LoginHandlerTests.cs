using System.IdentityModel.Tokens.Jwt;
using Application.UnitTests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.Login;

[TestFixture]
public class LoginHandlerTests : TestBase
{
    private LoginHandler _handler = null!;
    private IPasswordHashService _passwordHashService = null!;
    private IRedisService _redisService = null!;
    private ITokenService _tokenService = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _passwordHashService = new PasswordHashService();
        
        // Mock RedisService since Redis connection might not be available in tests
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        redisServiceMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((string?)null);
        _redisService = redisServiceMock.Object;
        
        var jwtSettings = new LetsTripTogether.InternalApi.Infrastructure.Configurations.JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440
        };
        
        _tokenService = new TokenService(Options.Create(jwtSettings), new JwtSecurityTokenHandler());
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        _handler = new LoginHandler(_passwordHashService, _redisService, _tokenService, _userRepository);
    }

    [Test]
    public async Task Handle_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var roleName = Roles.User;
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, roleName);
        await _roleRepository.AddOrUpdateAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var password = TestDataHelper.GenerateValidPassword();
        var passwordHash = _passwordHashService.HashPassword(password);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = email,
            Password = password
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Handle_WithInvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = TestDataHelper.GenerateRandomEmail(),
            Password = TestDataHelper.GenerateValidPassword()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.UnauthorizedException>();
    }

    [Test]
    public async Task Handle_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var roleName = Roles.User;
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, roleName);
        await _roleRepository.AddOrUpdateAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var correctPassword = TestDataHelper.GenerateValidPassword();
        var passwordHash = _passwordHashService.HashPassword(correctPassword);
        var userName = TestDataHelper.GenerateRandomName();
        var user = new LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new LoginCommand
        {
            Email = email,
            Password = TestDataHelper.GenerateValidPassword() // Different password
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.UnauthorizedException>();
    }
}
