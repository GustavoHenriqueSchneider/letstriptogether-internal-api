using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Controllers;

// TODO: usar Tasks (await/async)
// TODO: aplicar CQRS com usecases, mediator com mediatr, repository, DI e clean arc
// TODO: colocar tag de versionamento e descricoes para swagger
// TODO: definir retorno das rotas com classes de response e converter returns de erro em exception
[Authorize]
[ApiController]
[Route("api/v1/destinations")]
public class DestinationsController(AppDbContext context) : ControllerBase
{
    // TODO: remover esses dados da controller
    private const string GeoapifyApiKey = "7cfabde88e4341909197f829746da3f3";
    private const string GeoapifyApiBaseUrl = $"https://api.geoapify.com/v2/places?categories=tourism&&filter=rect:-73.99,-33.75,-34.79,5.27&lang=pt&apiKey={GeoapifyApiKey}";

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        // TODO: paginacao deve funcionar via parametros de query, por enquanto pega os mesmos 10 valores, precisa de offset
        const int pageSize = 10;

        var destinations = context.Destinations.OrderBy(x => x.CreatedAt).Take(pageSize).ToList();
        if (destinations.Count > 0)
        {
            return Ok(destinations.Select(x => new { x.Id, x.CreatedAt }).ToList());
        }
        
        var client = new HttpClient();
        var url = $"{GeoapifyApiBaseUrl}&limit={pageSize + 1}";
        
        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Erro ao buscar dados na Geoapify API");
        }
        
        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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

        context.Destinations.AddRange(newDestinations);
        context.SaveChanges();

        return Ok(newDestinations.Select(x => new { x.Id, x.CreatedAt }).ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<object> GetById(Guid id)
    {
        var destination = context.Destinations.SingleOrDefault(x => x.Id == id);

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
