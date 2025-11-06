using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;
using LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroup.Query.AdminGetGroupById;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace WebApi.UnitTests.Controllers.v1.Admin;

[TestFixture]
public class AdminGroupControllerTests
{
    private AdminGroupController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminGroupController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllGroups_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var response = new AdminGetAllGroupsResponse { Data = Enumerable.Empty<AdminGetAllGroupsResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<AdminGetAllGroupsQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllGroups(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetGroupById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var response = new AdminGetGroupByIdResponse();

        _mediatorMock.Setup(x => x.Send(
            It.Is<AdminGetGroupByIdQuery>(q => q.GroupId == groupId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetGroupById(groupId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
