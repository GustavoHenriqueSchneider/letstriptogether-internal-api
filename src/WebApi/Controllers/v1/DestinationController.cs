using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses;
using LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Destination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/destinations")]
public class DestinationController(
    IDestinationRepository destinationRepository) : ControllerBase
{
    [HttpGet("{destinationId:guid}")]
    public async Task<IActionResult> GetDestinationById([FromRoute] Guid destinationId, CancellationToken cancellationToken)
    {
        var destination = await destinationRepository.GetByIdAsync(destinationId, cancellationToken);

        if (destination is null)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        return Ok(new GetDestinationByIdResponse
        {
            Place = destination.Address,
            Description = destination.Description,
            Attractions = destination.Attractions.Select(a => new DestinationAttractionModel
            {
                Name = a.Name,
                Description = a.Description,
                Category = a.Category
            }),
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        });
    }
}
