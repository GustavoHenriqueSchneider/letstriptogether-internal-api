using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class NotificationService(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor,
    ILogger<NotificationService> logger) : INotificationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
    
    private void SetAuthentication(HttpRequestMessage httpRequestMessage)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.Request.Headers.ContainsKey("Authorization") != true)
        {
            return;
        }

        var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            httpRequestMessage.Headers.Add("Authorization", authorizationHeader);
        }
    }
    
    private static void SetRequestBody(HttpRequestMessage request, object body)
    {
        var json = JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
    }
    
    public async Task SendNotificationAsync(Guid userId, string eventName, 
        object data, CancellationToken cancellationToken)
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            var notificationData = new { userId, eventName, data, createdAt = DateTime.UtcNow };

            SetAuthentication(httpRequestMessage);
            SetRequestBody(httpRequestMessage, notificationData);
            
            var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Erro ao enviar notificação para userId {UserId} com evento {EventName}", userId, eventName);
        }
    }
}
