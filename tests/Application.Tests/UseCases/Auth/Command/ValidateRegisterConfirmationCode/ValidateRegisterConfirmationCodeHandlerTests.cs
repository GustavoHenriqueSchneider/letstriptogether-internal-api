using Application.Tests.Common;
using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;
using LetsTripTogether.InternalApi.Infrastructure.Services;
using Moq;
using NUnit.Framework;

namespace Application.Tests.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

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
        
        var jwtSettings = new LetsTripTogether.InternalApi.Infrastructure.Configurations.JsonWebTokenSettings
        {
            Issuer = "test-issuer",
            SecretKey = "test-secret-key-that-is-at-least-32-characters-long",
            AccessTokenValidityInMinutes = 60,
            RefreshTokenValidityInMinutes = 1440
        };
        
        _tokenService = new TokenService(Microsoft.Extensions.Options.Options.Create(jwtSettings), new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());
        
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);
    }

    [Test]
    public async Task Handle_WithValidCode_ShouldReturnToken()
    {
        // Arrange
        var email = TestDataHelper.GenerateRandomEmail();
        var name = TestDataHelper.GenerateRandomName();
        var code = "123456";
        var key = LetsTripTogether.InternalApi.Application.Helpers.KeyHelper.RegisterEmailConfirmation(email);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync(code);
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
        var providedCode = "654321";
        var key = LetsTripTogether.InternalApi.Application.Helpers.KeyHelper.RegisterEmailConfirmation(email);

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
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>();
    }

    [Test]
    public async Task Handle_WithNonExistentCode_ShouldThrowBadRequestException()
    {
        // Arrange
        var email = TestDataHelper.GenerateRandomEmail();
        var name = TestDataHelper.GenerateRandomName();
        var key = LetsTripTogether.InternalApi.Application.Helpers.KeyHelper.RegisterEmailConfirmation(email);

        var redisServiceMock = new Mock<IRedisService>();
        redisServiceMock.Setup(x => x.GetAsync(key)).ReturnsAsync((string?)null);
        _redisService = redisServiceMock.Object;
        _handler = new ValidateRegisterConfirmationCodeHandler(_redisService, _tokenService);

        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Email = email,
            Name = name,
            Code = "123456"
        };

        // Act & Assert
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<LetsTripTogether.InternalApi.Application.Common.Exceptions.BadRequestException>();
    }
}
