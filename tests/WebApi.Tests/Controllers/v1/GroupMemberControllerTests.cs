using Application.Common.Interfaces.Extensions;
using Application.UseCases.GroupMember.Command.RemoveGroupMemberById;
using Application.UseCases.GroupMember.Query.GetGroupMemberById;
using Application.UseCases.GroupMember.Query.GetOtherGroupMembersById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.Tests.Controllers.v1;

[TestFixture]
public class GroupMemberControllerTests
{
    private GroupMemberController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _controller = new GroupMemberController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task GetOtherGroupMembersById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new GetOtherGroupMembersByIdResponse { Data = Enumerable.Empty<GetOtherGroupMembersByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetOtherGroupMembersByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetOtherGroupMembersById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupMemberById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var response = new GetGroupMemberByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetGroupMemberByIdQuery>(q => q.GroupId == groupId && q.MemberId == memberId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetGroupMemberById(groupId, memberId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task RemoveGroupMemberById_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<RemoveGroupMemberByIdCommand>(c => c.GroupId == groupId && c.MemberId == memberId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveGroupMemberById(groupId, memberId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
