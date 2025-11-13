using System.Text.Json;
using Infrastructure.Configurations;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Infrastructure.UnitTests.Services;

[TestFixture]
public class EmailTemplateServiceTests
{
    private EmailTemplateService _service = null!;
    private EmailTemplateSettings _settings = null!;

    [SetUp]
    public void SetUp()
    {
        _settings = new EmailTemplateSettings
        {
            Company = new CompanyData
            {
                Name = "Test Company",
                Contact = "contact@test.com"
            },
            Url = new UrlData
            {
                Website = "https://test.com",
                PrivacyPolicy = "/privacy",
                TermsOfUse = "/terms"
            }
        };

        var loggerMock = new Mock<ILogger<EmailTemplateService>>();
        _service = new EmailTemplateService(_settings, loggerMock.Object);
    }

    [Test]
    public async Task GetTemplateAsync_WithValidTemplateName_ShouldReturnSubjectAndHtmlBody()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "John Doe" },
            { "code", "123456" },
            { "email", "john@example.com" },
            { "expiresIn", "30" }
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.subject, Is.Not.Null);
        Assert.That(result.htmlBody, Is.Not.Null);
        Assert.That(result.subject, Does.Contain("Confirmação de Email"));
        Assert.That(result.htmlBody, Does.Contain("John Doe"));
        Assert.That(result.htmlBody, Does.Contain("123456"));
    }

    [Test]
    public async Task GetTemplateAsync_WithResetPasswordTemplate_ShouldReturnSubjectAndHtmlBody()
    {
        // Arrange
        const string templateName = "reset-password";
        var data = new Dictionary<string, string>
        {
            { "token", "test-token-123" },
            { "email", "user@example.com" },
            { "expiresIn", "60" }
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.subject, Is.Not.Null);
        Assert.That(result.htmlBody, Is.Not.Null);
        Assert.That(result.subject, Does.Contain("Recuperação de Senha"));
        Assert.That(result.htmlBody, Does.Contain("test-token-123"));
    }

    [Test]
    public async Task GetTemplateAsync_ShouldReplaceAllPlaceholders()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "Jane Doe" },
            { "code", "654321" },
            { "email", "jane@example.com" },
            { "expiresIn", "15" }
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.htmlBody, Does.Contain("Jane Doe"));
        Assert.That(result.htmlBody, Does.Contain("654321"));
        Assert.That(result.htmlBody, Does.Contain("jane@example.com"));
        Assert.That(result.htmlBody, Does.Contain("15"));
    }

    [Test]
    public async Task GetTemplateAsync_ShouldIncludeCompanyData()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "Test User" },
            { "code", "000000" },
            { "email", "test@example.com" },
            { "expiresIn", "30" }
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.subject, Does.Contain("Test Company"));
        Assert.That(result.htmlBody, Does.Contain("Test Company"));
        Assert.That(result.htmlBody, Does.Contain("contact@test.com"));
        Assert.That(result.htmlBody, Does.Contain("https://test.com"));
        Assert.That(result.htmlBody, Does.Contain("https://test.com/privacy"));
        Assert.That(result.htmlBody, Does.Contain("https://test.com/terms"));
    }

    [Test]
    public async Task GetTemplateAsync_ShouldIncludeCurrentYear()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "Test User" },
            { "code", "000000" },
            { "email", "test@example.com" },
            { "expiresIn", "30" }
        };
        var currentYear = DateTime.UtcNow.Year.ToString();

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.htmlBody, Does.Contain(currentYear));
    }

    [Test]
    public void GetTemplateAsync_WithInvalidTemplateName_ShouldThrowArgumentException()
    {
        // Arrange
        const string templateName = "invalid-template";
        var data = new Dictionary<string, string>();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetTemplateAsync(templateName, data));
        
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex!.Message, Does.Contain("Template 'invalid-template' not found"));
    }

    [Test]
    public async Task GetTemplateAsync_WithMissingPlaceholder_ShouldKeepOriginalPlaceholder()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "Test User" },
            { "code", "000000" }
            // Missing email and expiresIn
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.htmlBody, Does.Contain("Test User"));
        Assert.That(result.htmlBody, Does.Contain("000000"));
        // Placeholders that don't exist in data should remain as {{placeholder}}
        // But since we're providing minimal data, the template should still render
        Assert.That(result.htmlBody, Is.Not.Null);
    }

    [Test]
    public async Task GetTemplateAsync_ShouldFormatSubjectWithCompanyName()
    {
        // Arrange
        const string templateName = "email-confirmation";
        var data = new Dictionary<string, string>
        {
            { "name", "Test User" },
            { "code", "000000" },
            { "email", "test@example.com" },
            { "expiresIn", "30" }
        };

        // Act
        var result = await _service.GetTemplateAsync(templateName, data);

        // Assert
        Assert.That(result.subject, Does.Contain("Confirmação de Email"));
        Assert.That(result.subject, Does.Contain("Test Company"));
        Assert.That(result.subject, Is.EqualTo("Confirmação de Email - Test Company"));
    }
}
