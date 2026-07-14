namespace Order.Api.Domain.Events;

public sealed record OrderCreatedDomainEvent(
    string OrderId,
    string CustomerId,
    int ItemCount) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
