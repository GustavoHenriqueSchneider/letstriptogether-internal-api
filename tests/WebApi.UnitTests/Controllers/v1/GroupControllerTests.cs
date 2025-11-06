using FluentAssertions;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Extensions;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Command.CreateGroup;
using LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetAllGroups;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

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
}
