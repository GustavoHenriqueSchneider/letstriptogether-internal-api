using Application.Common.Policies;
using Application.UseCases.Admin.AdminDestination.Query.AdminGetAllDestinations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers.v1.Admin;

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v{version:apiVersion}/admin/destinations")]
public class AdminDestinationController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Todos os Destinos (Admin)",
        Description = "Retorna uma lista paginada de todos os destinos do sistema. Requer permissões de administrador.")]
    [ProducesResponseType(typeof(AdminGetAllDestinationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminGetAllDestinations(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new AdminGetAllDestinationsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response = await mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
