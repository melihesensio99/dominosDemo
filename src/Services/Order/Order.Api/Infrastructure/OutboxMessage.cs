namespace Order.Api.Infrastructure;

public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset OccurredAt { get; set; }

    public DateTimeOffset? ProcessingAt { get; set; }

    public DateTimeOffset? LockedAt { get; set; }

    public int RetryCount { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTimeOffset? ProcessedAt { get; set; }

    public DateTimeOffset? FailedAt { get; set; }

    public string? Error { get; set; }
}
