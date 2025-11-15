using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using Application.Helpers;
using Application.UnitTests.Common;
using Application.UseCases.v1.Auth.Command.ValidateRegisterConfirmationCode;
using FluentAssertions;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

[TestFixture]
public class ValidateRegisterConfirmationCodeHandlerTests : TestBase
{
    private ValidateRegisterConfirmationCodeHandler _handler = null!;
    private IRedisService _redisService = null!;
    private ITokenService _tokenService = null!;

    [SetUp]
    public void SetUp()
    {
        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.DeleteAsync(It.IsAny<string>()))
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
        
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);
    }

    [Test]
    public async Task Handle_WithValidCode_ShouldReturnToken()
    {
        // Arrange
        var email = TestDataHelper.GenerateRandomEmail();
        var name = TestDataHelper.GenerateRandomName();
        var code = 123456;
        var key = KeyHelper.RegisterEmailConfirmation(email);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync(code.ToString());
        redisServiceMock.Setup(x => x.DeleteAsync(key)).Returns(Task.CompletedTask);
        _redisService = redisServiceMock.Object;
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);

        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = email,
            Name = name,
            Code = code
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        redisServiceMock.Verify(x => x.DeleteAsync(key), Times.Once);
    }

    [Test]
    public async Task Handle_WithInvalidCode_ShouldThrowBadRequestException()
    {
        // Arrange
        var email = TestDataHelper.GenerateRandomEmail();
        var name = TestDataHelper.GenerateRandomName();
        var storedCode = "123456";
        var providedCode = 654321;
        var key = KeyHelper.RegisterEmailConfirmation(email);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync(storedCode);
        _redisService = redisServiceMock.Object;
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);

        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = email,
            Name = name,
            Code = providedCode
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Test]
    public async Task Handle_WithNonExistentCode_ShouldThrowBadRequestException()
    {
        // Arrange
        var email = TestDataHelper.GenerateRandomEmail();
        var name = TestDataHelper.GenerateRandomName();
        var key = KeyHelper.RegisterEmailConfirmation(email);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync((string?)null);
        _redisService = redisServiceMock.Object;
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);

        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = email,
            Name = name,
            Code = 123456
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<BadRequestException>();
    }
}
