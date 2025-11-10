using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;
using Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetGroupDestinationVoteById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1.Admin;

namespace WebApi.Tests.Controllers.v1.Admin;

[TestFixture]
public class AdminGroupDestinationVoteControllerTests
{
    private AdminGroupDestinationVoteController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminGroupDestinationVoteController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllGroupDestinationVotesById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new AdminGetAllGroupDestinationVotesByIdResponse { Data = Enumerable.Empty<AdminGetAllGroupDestinationVotesByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetAllGroupDestinationVotesByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllGroupDestinationVotesById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupDestinationVoteById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var voteId = Guid.NewGuid();
        var response = new AdminGetGroupDestinationVoteByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupDestinationVoteByIdQuery>(q => q.GroupId == groupId && q.DestinationVoteId == voteId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupDestinationVoteById(groupId, voteId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
