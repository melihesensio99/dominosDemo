using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;
using Notification.Api.Abstractions.Realtime;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class OrderStatusChangedConsumer(
    MongoNotificationStore store,
    IRealtimeNotificationPublisher realtimePublisher)
    : IConsumer<OrderStatusChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderStatusChangedIntegrationEvent> context)
    {
        var evt = context.Message;
        var message = $"Order '{evt.OrderId}' status changed from '{evt.PreviousStatus}' to '{evt.Status}'.";

        var (_, isNew) = await store.AddAsync(
            evt.EventId,
            evt.CustomerId,
            message,
            "received",
            context.CancellationToken);

        if (isNew)
        {
            await realtimePublisher.NotifyOrderStatusChangedAsync(
                new OrderStatusChangedNotification(
                    evt.OrderId,
                    evt.CustomerId,
                    evt.Status,
                    evt.OccurredAt));
        }
    }
}
