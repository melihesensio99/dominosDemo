namespace Order.Api.Domain.Events;

public sealed record OrderStatusChangedDomainEvent(
    string OrderId,
    string CustomerId,
    OrderStatus PreviousStatus,
    OrderStatus NewStatus) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
