using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses.Admin.Destination;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Controllers.Admin;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[ApiController]
[Authorize(Policy = Policies.Admin)]
[Route("api/v1/admin/destinations")]
public class AdminDestinationController(
    IDestinationRepository destinationRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> AdminGetAllDestinations([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var (destinations, hits) = await destinationRepository.GetAllAsync(pageNumber, pageSize); 

        return Ok(new AdminGetAllDestinationsResponse
        {
            Data = destinations.Select(x => new AdminGetAllDestinationsResponseData
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }),
            Hits = hits
        });
    }
}

