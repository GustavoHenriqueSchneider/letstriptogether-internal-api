using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Responses;
using WebApi.DTOs.Responses.Destination;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: converter returns de erro em exception

[Authorize]
[ApiController]
[Route("api/v1/destinations")]
public class DestinationController(
    IGeoapifyService geoapifyService,
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var (destinations, hits) = await destinationRepository.GetAllAsync(pageNumber, pageSize); 

        if (destinations.Any())
        {
            return Ok(new GetAllDestinationsResponse
            {
                Data = destinations.Select(x => new GetAllDestinationsResponseData
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt
                }),
                Hits = hits
            });
        }

        try 
        {
            // TODO: implementar logica para verificar se destinos buscados ja nao estao no banco
            var newDestinations = await geoapifyService.GetNewDestinationsAsync(pageSize);

            await destinationRepository.AddRangeAsync(newDestinations);
            await unitOfWork.SaveAsync();

            return Ok(new GetAllDestinationsResponse
            {
                Data = newDestinations.Select(x => new GetAllDestinationsResponseData
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt
                }),
                Hits = newDestinations.Count
            });
        } 
        catch (HttpRequestException ex) when (ex.Message == GeoapifyService.ExceptionDefaultMessage) 
        {
            return StatusCode((int)ex.StatusCode!, new ErrorResponse(ex.Message));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var destination = await destinationRepository.GetByIdAsync(id);

        if (destination is null)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        return Ok(new GetDestinationByIdResponse
        {
            Address = destination.Address,
            Categories = destination.Categories,
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        });
    }
}
