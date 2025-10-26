using Microsoft.AspNetCore.Http.HttpResults;
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

    private readonly string _categories;
    private readonly string _filter;
    private readonly string _lang;
    private readonly string _apiKey;

    public GeoapifyService(IGeoapifyClient geoapifyClient, GeoapifyApiSettings geoapifyApiSettings)
    {
        _geoapifyClient = geoapifyClient.Client;
        _categories = geoapifyApiSettings.Categories;
        _filter = geoapifyApiSettings.Filter;
        _lang = geoapifyApiSettings.Lang;
        _apiKey = geoapifyApiSettings.ApiKey;
    }

    private string ApplyApiKeyOnPath(string path)
    {
        return path + $"&apiKey={_apiKey}";
    }

    public async Task<List<Destination>> GetNewDestinationsAsync(int pageSize = 10)
    {
        var path = $"places?categories={_categories}&filter={_filter}" +
            $"&lang={_lang}&limit={pageSize + 1}";

        var response = await _geoapifyClient.GetAsync(ApplyApiKeyOnPath(path));

        if (!response.IsSuccessStatusCode)
        {
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

        return destinations;
    }
}
