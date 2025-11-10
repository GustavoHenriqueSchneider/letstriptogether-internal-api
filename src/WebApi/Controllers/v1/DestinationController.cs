using Application.UseCases.Destination.Query.GetDestinationById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/destinations")]
public class DestinationController(IMediator mediator) : ControllerBase
{
    [HttpGet("{destinationId:guid}")]
    [SwaggerOperation(
        Summary = "Obter Destino por ID",
        Description = "Retorna os detalhes de um destino específico, incluindo informações sobre atrações.")]
    [ProducesResponseType(typeof(GetDestinationByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDestinationById(
        [FromRoute] Guid destinationId, 
        CancellationToken cancellationToken)
    {
        var query = new GetDestinationByIdQuery
        {
            DestinationId = destinationId
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
