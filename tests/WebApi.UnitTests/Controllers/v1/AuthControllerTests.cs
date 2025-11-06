using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

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
}
