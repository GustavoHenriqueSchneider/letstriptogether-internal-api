using Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;
using Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetGroupMatchById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1.Admin;

namespace WebApi.UnitTests.Controllers.v1.Admin;

[TestFixture]
public class AdminGroupMatchControllerTests
{
    private AdminGroupMatchController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminGroupMatchController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllGroupMatchesById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new AdminGetAllGroupMatchesByIdResponse { Data = Enumerable.Empty<AdminGetAllGroupMatchesByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetAllGroupMatchesByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllGroupMatchesById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupMatchById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var response = new AdminGetGroupMatchByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupMatchByIdQuery>(q => q.GroupId == groupId && q.MatchId == matchId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupMatchById(groupId, matchId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
