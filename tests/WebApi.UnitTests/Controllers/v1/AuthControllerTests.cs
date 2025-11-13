using Application.Common.Interfaces.Extensions;
using Application.UseCases.Auth.Command.Login;
using Application.UseCases.Auth.Command.Logout;
using Application.UseCases.Auth.Command.RefreshToken;
using Application.UseCases.Auth.Command.Register;
using Application.UseCases.Auth.Command.RequestResetPassword;
using Application.UseCases.Auth.Command.ResetPassword;
using Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;
using Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class AuthControllerTests
{
    private AuthController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;
    private Mock<IHttpContextExtensions> _httpContextExtensionsMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        _httpContextExtensionsMock = new Mock<IHttpContextExtensions>();
        
        _controller = new AuthController(
            _mediatorMock.Object,
            _currentUserMock.Object,
            _httpContextExtensionsMock.Object);
    }

    [Test]
    public async Task Login_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "ValidPass123!"
        };
        
        var response = new LoginResponse
        {
            AccessToken = "token",
            RefreshToken = "refresh"
        };

        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Logout_WithValidUser_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<LogoutCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<LogoutCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Register_WithValidCommand_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";
        _currentUserMock.Setup(x => x.GetEmail()).Returns(email);
        _currentUserMock.Setup(x => x.GetName()).Returns(name);
        
        var command = new RegisterCommand
        {
            Password = "ValidPass123!"
        };
        
        var response = new RegisterResponse
        {
            Id = Guid.NewGuid()
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<RegisterCommand>(c => c.Email == email && c.Name == name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RefreshToken_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var command = new RefreshTokenCommand
        {
            RefreshToken = "refresh-token"
        };
        
        var response = new RefreshTokenResponse
        {
            AccessToken = "new-token",
            RefreshToken = "new-refresh"
        };

        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RefreshToken(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task RequestResetPassword_WithValidCommand_ShouldReturnAccepted()
    {
        // Arrange
        var command = new RequestResetPasswordCommand
        {
            Email = "test@example.com"
        };
        
        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RequestResetPassword(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    [Test]
    public async Task ResetPassword_WithValidCommand_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bearerToken = "bearer-token";
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        _httpContextExtensionsMock.Setup(x => x.GetBearerToken()).Returns(bearerToken);
        
        var command = new ResetPasswordCommand
        {
            Password = "NewPass123!"
        };
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResetPassword(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<ResetPasswordCommand>(c => c.UserId == userId && c.BearerToken == bearerToken), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendRegisterConfirmationEmail_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var command = new SendRegisterConfirmationEmailCommand
        {
            Email = "test@example.com",
            Name = "Test User"
        };
        
        var response = new SendRegisterConfirmationEmailResponse
        {
            Token = "confirmation-token"
        };

        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SendRegisterConfirmationEmail(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ValidateRegisterConfirmationCode_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var email = "test@example.com";
        var name = "Test User";
        _currentUserMock.Setup(x => x.GetEmail()).Returns(email);
        _currentUserMock.Setup(x => x.GetName()).Returns(name);
        
        var command = new ValidateRegisterConfirmationCodeCommand
        {
            Code = 123456
        };
        
        var response = new ValidateRegisterConfirmationCodeResponse
        {
            Token = "validation-token"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<ValidateRegisterConfirmationCodeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ValidateRegisterConfirmationCode(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<ValidateRegisterConfirmationCodeCommand>(c => c.Email == email && c.Name == name), It.IsAny<CancellationToken>()), Times.Once);
    }
}
