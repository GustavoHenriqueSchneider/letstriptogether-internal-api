using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Command.AdminRemoveGroupMemberById;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetAllGroupMembersById;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace WebApi.UnitTests.Controllers.v1.Admin;

[TestFixture]
public class AdminGroupMemberControllerTests
{
    private AdminGroupMemberController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminGroupMemberController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllGroupMembersById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new AdminGetAllGroupMembersByIdResponse { Data = Enumerable.Empty<AdminGetAllGroupMembersByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetAllGroupMembersByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllGroupMembersById(groupId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupMemberById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var response = new AdminGetGroupMemberByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupMemberByIdQuery>(q => q.GroupId == groupId && q.MemberId == memberId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupMemberById(groupId, memberId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupMemberAllDestinationVotesById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var response = new AdminGetGroupMemberAllDestinationVotesByIdResponse { Data = Enumerable.Empty<AdminGetGroupMemberAllDestinationVotesByIdResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupMemberAllDestinationVotesByIdQuery>(q => q.GroupId == groupId && q.MemberId == memberId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupMemberAllDestinationVotesById(groupId, memberId, 1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task RemoveGroupMemberById_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var memberId = Guid.NewGuid();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminRemoveGroupMemberByIdCommand>(c => c.GroupId == groupId && c.MemberId == memberId),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AdminRemoveGroupMemberById(groupId, memberId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
