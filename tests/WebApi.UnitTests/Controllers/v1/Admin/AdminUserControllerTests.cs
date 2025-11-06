using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminCreateUser;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Command.AdminUpdateUserById;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetAllUsers;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminUser.Query.AdminGetUserById;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace WebApi.UnitTests.Controllers.v1.Admin;

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
}
