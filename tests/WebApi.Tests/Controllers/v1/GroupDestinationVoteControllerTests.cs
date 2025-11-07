using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.UpdateDestinationVoteById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Command.VoteAtDestinationForGroupId;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupDestinationVoteById;
using LetsTripTogether.InternalApi.Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace WebApi.Tests.Controllers.v1;

[TestFixture]
public class GroupDestinationVoteControllerTests
{
    private GroupDestinationVoteController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _controller = new GroupDestinationVoteController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task VoteAtDestinationForGroupId_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var command = new VoteAtDestinationForGroupIdCommand
        {
            DestinationId = Guid.NewGuid(),
            IsApproved = true
        };
        var response = new VoteAtDestinationForGroupIdResponse { Id = Guid.NewGuid() };

        _mediatorMock.Setup(x => x.Send(
            It.Is<VoteAtDestinationForGroupIdCommand>(c => c.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.VoteAtDestinationForGroupId(groupId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task UpdateDestinationVoteById_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var destinationVoteId = Guid.NewGuid();
        var command = new UpdateDestinationVoteByIdCommand { IsApproved = false };

        _mediatorMock.Setup(x => x.Send(
            It.Is<UpdateDestinationVoteByIdCommand>(c => c.GroupId == groupId && c.DestinationVoteId == destinationVoteId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateDestinationVoteById(groupId, destinationVoteId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task GetGroupMemberAllDestinationVotesById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new GetGroupMemberAllDestinationVotesByIdResponse { Data = Enumerable.Empty<GetGroupMemberAllDestinationVotesByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetGroupMemberAllDestinationVotesByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetGroupMemberAllDestinationVotesById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupDestinationVoteById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var destinationVoteId = Guid.NewGuid();
        var response = new GetGroupDestinationVoteByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetGroupDestinationVoteByIdQuery>(q => q.GroupId == groupId && q.DestinationVoteId == destinationVoteId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetGroupDestinationVoteById(groupId, destinationVoteId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
