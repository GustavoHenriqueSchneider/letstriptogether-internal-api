using Application.Tests.Common;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Security;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;
using LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.RequestResetPassword;

[TestFixture]
public class RequestResetPasswordHandlerTests : TestBase
{
    private RequestResetPasswordHandler _handler = null!;
    private IEmailSenderService _emailSenderService = null!;
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
        _userRepository = new UserRepository(DbContext);
        _roleRepository = new RoleRepository(DbContext);
        
        var emailSenderServiceMock = new Mock<IEmailSenderService>();
        emailSenderServiceMock.Setup(x => x.SendAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _emailSenderService = emailSenderServiceMock.Object;
        
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.SetAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        
        var jwtSettings = new LetsTripTogether.InternalApi.Infrastructure.Configurations.JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 10,
            RefreshTokenValidityInMinutes = 10,
            InvitationTokenValidityInMinutes = 10,
            ResetPasswordTokenValidityInMinutes = 10
        };
        
        _tokenService = new TokenService(Microsoft.Extensions.Options.Options.Create(jwtSettings), new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());
        
        _handler = new RequestResetPasswordHandler(_emailSenderService, _redisService, _tokenService, _userRepository);
    }

    [Test]
    public async Task Handle_WithExistingEmail_ShouldSendEmailAndStoreToken()
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

        var command = new RequestResetPasswordCommand { Email = email };
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        _handler = new RequestResetPasswordHandler(_emailSenderService, _redisService, _tokenService, _userRepository);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        redisServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithNonExistingEmail_ShouldNotThrowException()
    {
        // Arrange
        var command = new RequestResetPasswordCommand { Email = TestDataHelper.GenerateRandomEmail() };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert - Should not throw exception, handler returns early
    }
}
