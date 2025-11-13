using Application.Common.Interfaces.Extensions;
using Application.UseCases.GroupInvitation.Command.CancelActiveGroupInvitation;
using Application.UseCases.GroupInvitation.Command.CreateGroupInvitation;
using Application.UseCases.GroupInvitation.Query.GetActiveGroupInvitation;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class GroupInvitationControllerTests
{
    private GroupInvitationController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _controller = new GroupInvitationController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task CreateGroupInvitation_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new CreateGroupInvitationResponse { Token = "fake-token-jwt" };

        _mediatorMock.Setup(x => x.Send(
            It.Is<CreateGroupInvitationCommand>(c => c.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateGroupInvitation(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task GetActiveGroupInvitation_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new GetActiveGroupInvitationResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetActiveGroupInvitationQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetActiveGroupInvitation(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task CancelActiveGroupInvitation_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<CancelActiveGroupInvitationCommand>(c => c.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CancelActiveGroupInvitation(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
