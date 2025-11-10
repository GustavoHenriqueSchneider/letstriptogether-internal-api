using Application.UseCases.Admin.AdminUser.Command.AdminAnonymizeUserById;
using Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using Application.UseCases.Admin.AdminUser.Command.AdminDeleteUserById;
using Application.UseCases.Admin.AdminUser.Command.AdminSetUserPreferencesByUserId;
using Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;
using Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;
using Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1.Admin;

namespace WebApi.Tests.Controllers.v1.Admin;

[TestFixture]
public class AdminUserControllerTests
{
    private AdminUserController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminUserController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllUsers_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var response = new AdminGetAllUsersResponse { Data = Enumerable.Empty<AdminGetAllUsersResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<AdminGetAllUsersQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllUsers(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetUserById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var response = new AdminGetUserByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetUserByIdQuery>(q => q.UserId == userId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetUserById(userId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task CreateUser_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var command = new AdminCreateUserCommand();
        var response = new AdminCreateUserResponse();

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<AdminCreateUserCommand>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminCreateUser(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task UpdateUserById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AdminUpdateUserByIdCommand { UserId = userId };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminUpdateUserByIdCommand>(c => c.UserId == userId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AdminUpdateUserById(userId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task AdminDeleteUserById_WithValidUserId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminDeleteUserByIdCommand>(c => c.UserId == userId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AdminDeleteUserById(userId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<AdminDeleteUserByIdCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AdminAnonymizeUserById_WithValidUserId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminAnonymizeUserByIdCommand>(c => c.UserId == userId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AdminAnonymizeUserById(userId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<AdminAnonymizeUserByIdCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AdminSetUserPreferencesByUserId_WithValidCommand_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AdminSetUserPreferencesByUserIdCommand
        {
            Food = new List<string> { "food.restaurant" },
            Culture = new List<string> { "culture.museum" }
        };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminSetUserPreferencesByUserIdCommand>(c => c.UserId == userId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AdminSetUserPreferencesByUserId(userId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<AdminSetUserPreferencesByUserIdCommand>(c => c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }
}
