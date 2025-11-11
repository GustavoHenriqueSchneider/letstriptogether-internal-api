namespace Application.Common.Interfaces.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string eventName, object data, CancellationToken cancellationToken);
}
