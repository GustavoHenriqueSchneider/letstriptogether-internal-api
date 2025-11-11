using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Tests.Common;
using Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;
using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Security;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Users;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.SendRegisterConfirmationEmail;

[TestFixture]
public class SendRegisterConfirmationEmailHandlerTests : TestBase
{
    private SendRegisterConfirmationEmailHandler _handler = null!;
    private IEmailSenderService _emailSenderService = null!;
    private IRandomCodeGeneratorService _randomCodeGeneratorService = null!;
    private IRedisService _redisService = null!;
    private ITokenService _tokenService = null!;
    private UserRepository _userRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await CleanDatabaseAsync();
        
        _userRepository = new UserRepository(DbContext);
        _randomCodeGeneratorService = new RandomCodeGeneratorService();
        
        var emailSenderServiceMock = new Mock<IEmailSenderService>();
        emailSenderServiceMock.Setup(x => x.SendAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<Dictionary<string, string>>(), 
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
        
        var jwtSettings = new JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440
        };
        
        var loggerMock = new Mock<ILogger<ITokenService>>();
        _tokenService = new TokenService(Microsoft.Extensions.Options.Options.Create(jwtSettings), new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler(), loggerMock.Object);
        
        _handler = new SendRegisterConfirmationEmailHandler(
            _emailSenderService, 
            _randomCodeGeneratorService, 
            _redisService, 
            _tokenService, 
            _userRepository);
    }

    [Test]
    public async Task Handle_WithNewEmail_ShouldReturnToken()
    {
        // Arrange
        var command = new SendRegisterConfirmationEmailCommand
        {
            Email = TestDataHelper.GenerateRandomEmail(),
            Name = TestDataHelper.GenerateRandomName()
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task Handle_WithExistingEmail_ShouldThrowConflictException()
    {
        // Arrange
        var role = new Role();
        typeof(Role).GetProperty("Name")!.SetValue(role, Roles.User);
        var roleRepository = new RoleRepository(DbContext);
        await roleRepository.AddAsync(role, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var email = TestDataHelper.GenerateRandomEmail();
        var passwordHash = new PasswordHashService().HashPassword(TestDataHelper.GenerateValidPassword());
        var userName = TestDataHelper.GenerateRandomName();
        var user = new Domain.Aggregates.UserAggregate.Entities.User(userName, email, passwordHash, role);
        await _userRepository.AddAsync(user, CancellationToken.None);
        await DbContext.SaveChangesAsync();

        var command = new SendRegisterConfirmationEmailCommand
        {
            Email = email,
            Name = TestDataHelper.GenerateRandomName()
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<ConflictException>();
    }
}
