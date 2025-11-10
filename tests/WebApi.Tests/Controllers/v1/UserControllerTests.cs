using Application.Common.Interfaces.Extensions;
using Application.UseCases.User.Command.AnonymizeCurrentUser;
using Application.UseCases.User.Command.DeleteCurrentUser;
using Application.UseCases.User.Command.SetCurrentUserPreferences;
using Application.UseCases.User.Command.UpdateCurrentUser;
using Application.UseCases.User.Query.GetCurrentUser;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.Tests.Controllers.v1;

[TestFixture]
public class UserControllerTests
{
    private UserController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        _controller = new UserController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task GetCurrentUser_WithValidUserId_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var query = new GetCurrentUserQuery { UserId = userId };
        var response = new GetCurrentUserResponse();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetCurrentUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetCurrentUser(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task UpdateCurrentUser_WithValidCommand_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var command = new UpdateCurrentUserCommand
        {
            Name = "Updated Name"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateCurrentUserCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateCurrentUser(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<UpdateCurrentUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteCurrentUser_WithValidUserId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteCurrentUserCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteCurrentUser(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<DeleteCurrentUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AnonymizeCurrentUser_WithValidUserId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<AnonymizeCurrentUserCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AnonymizeCurrentUser(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<AnonymizeCurrentUserCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SetCurrentUserPreferences_WithValidCommand_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var command = new SetCurrentUserPreferencesCommand
        {
            Food = new List<string> { "food.restaurant" },
            Culture = new List<string> { "culture.museum" }
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<SetCurrentUserPreferencesCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SetCurrentUserPreferences(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<SetCurrentUserPreferencesCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
