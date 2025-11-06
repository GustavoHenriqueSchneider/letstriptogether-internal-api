using FluentAssertions;
using LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;
using LetsTripTogether.InternalApi.WebApi.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace WebApi.UnitTests.Controllers.v1;

[TestFixture]
public class DestinationControllerTests
{
    private DestinationController _controller = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new DestinationController(_mediatorMock.Object);
    }

    [Test]
    public async Task GetDestinationById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var destinationId = Guid.NewGuid();
        var response = new GetDestinationByIdResponse
        {
            Place = "Test Place",
            Description = "Test Description",
            Attractions = new List<DestinationAttractionModel>()
        };

        _mediatorMock.Setup(x => x.Send(
            It.Is<GetDestinationByIdQuery>(q => q.DestinationId == destinationId),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetDestinationById(destinationId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }
}
