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
[Tags("Destinos")]
public class DestinationController(
    IGeoapifyService geoapifyService,
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    ///  Busca todos os destinos.
    /// </summary>
    /// <remarks>
    /// Retorna uma lista de todos os destinos ordenado por paginação.
    /// </remarks>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retorna lista paginada de todos os destinos</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetAllDestinationsResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllDestinations([FromQuery] int pageNumber = 1,
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
    /// <summary>
    /// Busca um destino pelo Id.
    /// </summary>
    /// <param name="destinationId">Retorna o Guid do destino a ser buscado</param>
    /// <response code="200">Retorna o destino buscado pelo Id</response>
    /// <response code="401">Usuário não autorizado(Token inválido ou vencido)</response>
    /// <response code="404">Destino não encontrado</response>
    [HttpGet("{destinationId:guid}")]
    [ProducesResponseType(typeof(GetDestinationByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDestinationById([FromRoute] Guid destinationId)
    {
        var destination = await destinationRepository.GetByIdAsync(destinationId);

        if (destination is null)
        {
            return NotFound(new ErrorResponse("Destination not found."));
        }

        return Ok(new GetDestinationByIdResponse
        {
            Address = destination.Address,
            Categories = destination.Categories.ToList(),
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        });
    }
}
