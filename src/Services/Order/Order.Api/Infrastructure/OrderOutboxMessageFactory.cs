using System.Text.Json;
using Inventory.Contracts.IntegrationEvents.Order;
using Order.Api.Domain.Events;

namespace Order.Api.Infrastructure;

public static class OrderOutboxMessageFactory
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IEnumerable<OutboxMessage> Create(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            switch (domainEvent)
            {
                case OrderCreatedDomainEvent created:
                {
                    var integrationEvent = new OrderCreatedIntegrationEvent(
                        created.OrderId,
                        created.CustomerId,
                        created.ItemCount,
                        "pending");

                    yield return CreateMessage("order.created", created.OccurredAt, integrationEvent);
                    break;
                }
                case OrderCancelledDomainEvent cancelled:
                {
                    var integrationEvent = new OrderCancelledIntegrationEvent(
                        cancelled.OrderId,
                        cancelled.CustomerId,
                        "cancelled");

                    yield return CreateMessage("order.cancelled", cancelled.OccurredAt, integrationEvent);
                    break;
                }
                case OrderStatusChangedDomainEvent statusChanged:
                {
                    var integrationEvent = new OrderStatusChangedIntegrationEvent(
                        statusChanged.OrderId,
                        statusChanged.CustomerId,
                        statusChanged.PreviousStatus.ToString().ToLowerInvariant(),
                        statusChanged.NewStatus.ToString().ToLowerInvariant());

                    yield return CreateMessage("order.status-changed", statusChanged.OccurredAt, integrationEvent);
                    break;
                }
            }
        }
    }

    private static OutboxMessage CreateMessage(string type, DateTimeOffset occurredAt, object payload) =>
        new()
        {
            Type = type,
            OccurredAt = occurredAt,
            Payload = JsonSerializer.Serialize(payload, JsonOptions),
        };
}
