using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApi.Models;
using WebApi.Persistence.Interfaces;
using WebApi.Repositories.Interfaces;

namespace WebApi.Controllers;

// TODO: aplicar CQRS com usecases, mediator com mediatr e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: definir retorno das rotas com classes de response e converter returns de erro em exception

[Authorize]
[ApiController]
[Route("api/v1/destinations")]
public class DestinationController(
    IDestinationRepository destinationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    // TODO: remover esses dados da controller
    private const string GeoapifyApiKey = "7cfabde88e4341909197f829746da3f3";
    private const string GeoapifyApiBaseUrl = $"https://api.geoapify.com/v2/places?categories=tourism&&filter=rect:-73.99,-33.75,-34.79,5.27&lang=pt&apiKey={GeoapifyApiKey}";

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var (destinations, hits) = await destinationRepository.GetAllAsync(pageNumber, pageSize); 

        if (destinations.Any())
        {
            return Ok(destinations.Select(x => new { x.Id, x.CreatedAt }).ToList());
        }
        
        // TODO: httpclient deve ser criado via DI
        var client = new HttpClient();
        var url = $"{GeoapifyApiBaseUrl}&limit={pageSize + 1}";
        
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Erro ao buscar dados na Geoapify API");
        }
        
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var features = doc.RootElement.GetProperty("features");

        var newDestinations = new List<Destination>();

        newDestinations.AddRange(features.EnumerateArray().Select(x =>
        {
            var properties = x.GetProperty("properties");
            return new Destination
            {
                Id = Guid.NewGuid(),
                Address = properties.GetProperty("formatted").GetString() ?? "Sem endereço",
                Categories = properties.GetProperty("categories").EnumerateArray()
                    .Select(c => c.GetString() ?? string.Empty)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList() ?? [],
                CreatedAt = DateTime.UtcNow
            };
        }));

        await destinationRepository.AddRangeAsync(newDestinations);
        await unitOfWork.SaveAsync();

        return Ok(newDestinations.Select(x => new { x.Id, x.CreatedAt }).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var destination = await destinationRepository.GetByIdAsync(id);

        if (destination is null)
        {
            return NotFound("Destination not found.");
        }

        return Ok(new
        {
            destination.Address,
            destination.Categories,
            destination.CreatedAt,
            destination.UpdatedAt
        });
    }
}
