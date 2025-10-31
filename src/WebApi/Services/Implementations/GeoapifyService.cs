using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using WebApi.Clients.Interfaces;
using WebApi.Configurations;
using WebApi.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class GeoapifyService : IGeoapifyService
{
    public const string ExceptionDefaultMessage = "GEOAPIFY ERROR";

    private readonly HttpClient _geoapifyClient;
    private readonly ILogger<GeoapifyService> _logger;

    private readonly string _categories;
    private readonly string _filter;
    private readonly string _lang;
    private readonly string _apiKey;

    public GeoapifyService(IGeoapifyClient geoapifyClient, GeoapifyApiSettings geoapifyApiSettings, ILogger<GeoapifyService> logger)
    {
        _geoapifyClient = geoapifyClient.Client;
        _categories = geoapifyApiSettings.Categories;
        _filter = geoapifyApiSettings.Filter;
        _lang = geoapifyApiSettings.Lang;
        _apiKey = geoapifyApiSettings.ApiKey;
        _logger = logger;
    }

    private string ApplyApiKeyOnPath(string path)
    {
        return path + $"&apiKey={_apiKey}";
    }

    public async Task<List<Destination>> GetNewDestinationsAsync(int pageSize = 10)
    {
        var path = $"places?categories={_categories}&filter={_filter}" +
            $"&lang={_lang}&limit={pageSize + 1}";

        try
        {
            _logger.LogInformation("Fetching destinations from Geoapify with pageSize {PageSize}", pageSize);
            var response = await _geoapifyClient.GetAsync(ApplyApiKeyOnPath(path));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Geoapify returned status {StatusCode}", response.StatusCode);
                throw new HttpRequestException(ExceptionDefaultMessage, null, response.StatusCode);
            }

            // TODO: melhorar esse processo de parse do json
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var features = doc.RootElement.GetProperty("features");

            var destinations = new List<Destination>();

            destinations.AddRange(features.EnumerateArray().Select(x =>
            {
                var properties = x.GetProperty("properties");
                var address = properties.GetProperty("formatted").GetString() ?? "Sem endereço";

                var categories = properties.GetProperty("categories").EnumerateArray()
                    .Select(c => c.GetString() ?? string.Empty)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList() ?? [];

                return new Destination(address, categories);
            }));

            _logger.LogInformation("Found {Count} destinations from Geoapify", destinations.Count);
            return destinations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching destinations from Geoapify");
            throw;
        }
    }
}
