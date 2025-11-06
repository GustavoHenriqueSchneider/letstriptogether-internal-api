using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.AcceptInvitation;
using LetsTripTogether.InternalApi.Application.UseCases.Invitation.Command.RefuseInvitation;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

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
        result.Should().BeOfType<OkResult>();
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
        result.Should().BeOfType<OkResult>();
    }
}
