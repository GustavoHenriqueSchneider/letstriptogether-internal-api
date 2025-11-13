using Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetAllGroupInvitationsByGroupId;
using Application.UseCases.Admin.AdminGroupInvitation.Query.AdminGetGroupInvitationById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1.Admin;

namespace WebApi.UnitTests.Controllers.v1.Admin;

[TestFixture]
public class AdminGroupInvitationsControllerTests
{
    private AdminGroupInvitationsController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminGroupInvitationsController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllGroupInvitationsByGroupId_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new AdminGetAllGroupInvitationsByGroupIdResponse { Data = Enumerable.Empty<AdminGetAllGroupInvitationsByGroupIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetAllGroupInvitationsByGroupIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllGroupInvitationsByGroupId(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupInvitationById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var response = new AdminGetGroupInvitationByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupInvitationByIdQuery>(q => q.GroupId == groupId && q.InvitationId == invitationId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupInvitationById(groupId, invitationId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
