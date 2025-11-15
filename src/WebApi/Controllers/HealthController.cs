using Application.UseCases.Health.Query.GetHealth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Health Check",
        Description = "Verifica o status de saúde da API, incluindo verificações de banco de dados e Redis.")]
    [ProducesResponseType(typeof(GetHealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetHealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var query = new GetHealthQuery();
        var response = await mediator.Send(query, cancellationToken);

        return response.Status == nameof(HealthStatus.Healthy).ToLowerInvariant()
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
