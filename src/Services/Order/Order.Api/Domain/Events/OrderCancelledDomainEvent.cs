namespace Order.Api.Domain.Events;

public sealed record OrderCancelledDomainEvent(
    string OrderId,
    string CustomerId) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
