using System.Collections.Concurrent;

namespace Notification.Api;

public sealed class NotificationStore
{
    private readonly ConcurrentDictionary<string, NotificationDto> notifications = new();

    public IReadOnlyCollection<NotificationDto> GetAll() =>
        notifications.Values.OrderByDescending(item => item.CreatedAt).ToArray();

    public NotificationDto? GetById(string id) =>
        notifications.TryGetValue(id, out var notification) ? notification : null;

    public NotificationDto Add(string recipientId, string message, string status = "queued")
    {
        var id = Guid.NewGuid().ToString("N");
        var notification = new NotificationDto(id, recipientId, message, status, DateTimeOffset.UtcNow);
        notifications[id] = notification;
        return notification;
    }
}

public sealed record NotificationDto(
    string Id,
    string RecipientId,
    string Message,
    string Status,
    DateTimeOffset CreatedAt);
