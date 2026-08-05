namespace Notification.Api.Infrastructure;

public sealed class NotificationDocument
{
    public string Id { get; set; } = string.Empty;

    public Guid EventId { get; set; }

    public string RecipientId { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Status { get; set; } = "queued";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
