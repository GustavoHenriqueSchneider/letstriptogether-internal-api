using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.Group.Command.CreateGroup;
using Application.UseCases.v1.Group.Command.DeleteGroupById;
using Application.UseCases.v1.Group.Command.LeaveGroupById;
using Application.UseCases.v1.Group.Command.UpdateGroupById;
using Application.UseCases.v1.Group.Query.GetAllGroups;
using Application.UseCases.v1.Group.Query.GetGroupById;
using Application.UseCases.v1.Group.Query.GetNotVotedDestinationsByMemberOnGroup;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class GroupControllerTests
{
    private GroupController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        _controller = new GroupController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task GetAllGroups_WithValidParameters_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var query = new GetAllGroupsQuery { UserId = userId };
        var response = new GetAllGroupsResponse();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllGroupsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllGroups(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task CreateGroup_WithValidCommand_ShouldReturnCreated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var command = new CreateGroupCommand
        {
            Name = "Test Group",
            TripExpectedDate = DateTime.UtcNow.AddDays(30)
        };
        
        var response = new CreateGroupResponse { Id = Guid.NewGuid() };

        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateGroup(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task GetGroupById_WithValidGroupId_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var response = new GetGroupByIdResponse();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetGroupByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetGroupById(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<GetGroupByIdQuery>(q => q.GroupId == groupId && q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateGroupById_WithValidCommand_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var command = new UpdateGroupByIdCommand
        {
            Name = "Updated Group Name"
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateGroupByIdCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateGroupById(groupId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<UpdateGroupByIdCommand>(c => c.GroupId == groupId && c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteGroupById_WithValidGroupId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteGroupByIdCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGroupById(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<DeleteGroupByIdCommand>(c => c.GroupId == groupId && c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task LeaveGroupById_WithValidGroupId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);

        _mediatorMock.Setup(x => x.Send(It.IsAny<LeaveGroupByIdCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.LeaveGroupById(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<LeaveGroupByIdCommand>(c => c.GroupId == groupId && c.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetNotVotedDestinationsByMemberOnGroup_WithValidParameters_ShouldReturnOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        var response = new GetNotVotedDestinationsByMemberOnGroupResponse();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetNotVotedDestinationsByMemberOnGroupQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetNotVotedDestinationsByMemberOnGroup(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.Is<GetNotVotedDestinationsByMemberOnGroupQuery>(q => q.GroupId == groupId && q.UserId == userId && q.PageNumber == 1 && q.PageSize == 10), It.IsAny<CancellationToken>()), Times.Once);
    }
}
