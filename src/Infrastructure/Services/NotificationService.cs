using System.Net.Http.Json;
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class NotificationService(
    HttpClient httpClient,
    ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendNotificationAsync(Guid userId, string eventName, 
        object data, CancellationToken cancellationToken)
    {
        try
        {
            var notificationRequest = new { userId, eventName, data, createdAt = DateTime.UtcNow };

            var response = await httpClient.PostAsJsonAsync(string.Empty, 
                notificationRequest, cancellationToken);
            
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Erro ao enviar notificação para userId {UserId} com evento {EventName}", userId, eventName);
        }
    }
}
