using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Common.Interfaces.Services;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly Dictionary<string, string> _templatesConfig;
    private readonly EmailTemplateSettings _settings;
    private readonly Assembly _assembly;
    private readonly ILogger<EmailTemplateService> _logger;
    private const string TemplatesNamespace = "Infrastructure.Templates.EmailTemplates";
    private const string TemplatesConfigFile = "templates.json";

    public EmailTemplateService(IOptions<EmailTemplateSettings> settings, ILogger<EmailTemplateService> logger)
    {
        _settings = settings.Value;
        _assembly = Assembly.GetExecutingAssembly();
        _logger = logger;
        _templatesConfig = LoadTemplatesConfig();
    }

    public async Task<(string subject, string htmlBody)> 
        GetTemplateAsync(string templateName, Dictionary<string, string> data)
    {
        if (!_templatesConfig.TryGetValue(templateName, out var subject))
        {
            throw new ArgumentException($"Template '{templateName}' not found.");
        }

        var templateResourceName = $"{TemplatesNamespace}.{templateName}.html";
        var templateContent = await ReadEmbeddedResourceAsync(templateResourceName);
        
        var allData = new Dictionary<string, string>(data)
        {
            { "subject", subject },
            { "companyName", _settings.Company.Name },
            { "companyEmail", _settings.Company.Contact },
            { "privacyPolicyUrl", _settings.Url.PrivacyPolicy },
            { "termsOfUseUrl", _settings.Url.TermsOfUse },
            { "websiteUrl", _settings.Url.Website },
            { "currentYear", DateTime.UtcNow.Year.ToString() }
        };
        
        var renderedContent = ReplacePlaceholders(templateContent, allData);
        return (subject: $"{subject} - {_settings.Company.Name}", renderedContent);
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> data)
    {
        return Regex.Replace(template, @"\{\{(\w+)\}\}", match =>
        {
            var placeholder = match.Groups[1].Value;
            return data.TryGetValue(placeholder, out var value) ? value : match.Value;
        });
    }

    private Dictionary<string, string> LoadTemplatesConfig()
    {
        var configResourceName = $"{TemplatesNamespace}.{TemplatesConfigFile}";
        var jsonContent = ReadEmbeddedResourceAsync(configResourceName).GetAwaiter().GetResult();
        
        var config = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent, 
            new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return config ?? new Dictionary<string, string>();
    }

    private async Task<string> ReadEmbeddedResourceAsync(string resourceName)
    {
        await using var stream = _assembly.GetManifestResourceStream(resourceName);
        
        if (stream is null)
        {
            var availableResources = _assembly.GetManifestResourceNames();
            _logger.LogError("Recurso embutido '{ResourceName}' não encontrado. Recursos disponíveis: {AvailableResources}", 
                resourceName, string.Join(", ", availableResources));
            throw new FileNotFoundException(
                $"Recurso embutido '{resourceName}' não encontrado.");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
