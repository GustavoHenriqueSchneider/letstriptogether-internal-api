using Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using WebApi.Controllers.v1.Admin;

namespace WebApi.UnitTests.Controllers.v1.Admin;

[TestFixture]
public class AdminDestinationControllerTests
{
    private AdminDestinationController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AdminDestinationController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetAllDestinations_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var response = new AdminGetAllDestinationsResponse { Data = Enumerable.Empty<AdminGetAllDestinationsResponseData>(), Hits = 0 };

        _mediatorMock.Setup(x => x.Send(
            It.IsAny<AdminGetAllDestinationsQuery>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AdminGetAllDestinations(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}
