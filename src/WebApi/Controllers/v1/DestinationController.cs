using LetsTripTogether.InternalApi.Application.UseCases.Destination.Query.GetDestinationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers.v1;

// TODO: colocar tag de versionamento e descricoes para swagger

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/destinations")]
public class DestinationController(IMediator mediator) : ControllerBase
{
    [HttpGet("{destinationId:guid}")]
    public async Task<IActionResult> GetDestinationById([FromRoute] Guid destinationId, CancellationToken cancellationToken)
    {
        var query = new GetDestinationByIdQuery
        {
            DestinationId = destinationId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
