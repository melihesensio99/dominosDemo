namespace Notification.Api.Infrastructure;

public sealed class NotificationDocument
{
    public string Id { get; set; } = string.Empty;

    public string EventId { get; set; } = string.Empty;

    public string RecipientId { get; set; } = string.Empty;

    public string Type { get; set; } = "general";

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Status { get; set; } = "queued";

    public string? StockKey { get; set; }

    public int? Available { get; set; }

    public int? ReorderLevel { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
