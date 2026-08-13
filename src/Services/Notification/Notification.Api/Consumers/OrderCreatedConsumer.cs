using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;
using Notification.Api.Abstractions.Realtime;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class OrderCreatedConsumer(
    MongoNotificationStore store,
    IRealtimeNotificationPublisher realtimePublisher)
    : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        var message = $"Order '{evt.OrderId}' was created for customer '{evt.CustomerId}' with {evt.ItemCount} items.";
        var (_, isNew) = await store.AddAsync(
            evt.EventId,
            evt.CustomerId,
            message,
            "received",
            context.CancellationToken);

        if (isNew)
        {
            await realtimePublisher.NotifyNewOrderAsync(
                new NewOrderNotification(
                    evt.OrderId,
                    evt.CustomerId,
                    evt.Status,
                    evt.ItemCount,
                    evt.OccurredAt));
        }
    }
}
