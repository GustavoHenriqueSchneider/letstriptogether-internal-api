using Application.Common.Interfaces.Extensions;
using Application.UseCases.v1.GroupMatch.Command.RemoveGroupMatchById;
using Application.UseCases.v1.GroupMatch.Query.GetAllGroupMatchesById;
using Application.UseCases.v1.GroupMatch.Query.GetGroupMatchById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class GroupMatchControllerTests
{
    private GroupMatchController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IApplicationUserContextExtensions> _currentUserMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _currentUserMock = new Mock<IApplicationUserContextExtensions>();
        
        var userId = Guid.NewGuid();
        _currentUserMock.Setup(x => x.GetId()).Returns(userId);
        
        _controller = new GroupMatchController(_mediatorMock.Object, _currentUserMock.Object);
    }

    [Test]
    public async Task GetAllGroupMatchesById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new GetAllGroupMatchesByIdResponse { Data = Enumerable.Empty<GetAllGroupMatchesByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetAllGroupMatchesByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllGroupMatchesById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupMatchById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var response = new GetGroupMatchByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetGroupMatchByIdQuery>(q => q.GroupId == groupId && q.MatchId == matchId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetGroupMatchById(groupId, matchId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task RemoveGroupMatchById_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var matchId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<RemoveGroupMatchByIdCommand>(c => c.GroupId == groupId && c.MatchId == matchId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RemoveGroupMatchById(groupId, matchId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
