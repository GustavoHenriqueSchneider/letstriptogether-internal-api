using Application.Common.Interfaces.Extensions;
using Application.UseCases.Invitation.Command.AcceptInvitation;
using Application.UseCases.Invitation.Command.RefuseInvitation;
using Application.UseCases.Invitation.Query.GetInvitationDetails;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class InvitationControllerTests
{
    private InvitationController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _controller = new InvitationController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task AcceptInvitation_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var command = new AcceptInvitationCommand
        {
            UserId = Guid.NewGuid(),
            Token = "fake-token-jwt"
        };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<AcceptInvitationCommand>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AcceptInvitation(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task RefuseInvitation_WithValidCommand_ShouldReturnOk()
    {
        // Arrange
        var command = new RefuseInvitationCommand
        {
            UserId = Guid.NewGuid(),
            Token = "fake-token-jwt"
        };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<RefuseInvitationCommand>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RefuseInvitation(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task GetInvitationDetails_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        var token = "valid-token";
        var response = new GetInvitationDetailsResponse
        {
            CreatedBy = "Test User",
            GroupName = "Test Group",
            IsActive = true
        };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<GetInvitationDetailsQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetInvitationDetails(token, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(response);
        _mediatorMock.Verify(x => x.Send(
            It.Is<GetInvitationDetailsQuery>(q => q.Token == token),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
