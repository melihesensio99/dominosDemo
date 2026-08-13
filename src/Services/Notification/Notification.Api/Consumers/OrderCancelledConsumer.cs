using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class OrderCancelledConsumer(MongoNotificationStore store)
    : IConsumer<OrderCancelledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCancelledIntegrationEvent> context)
    {
        var evt = context.Message;
        var message = $"Order '{evt.OrderId}' was cancelled for customer '{evt.CustomerId}'.";
        await store.AddAsync(
            evt.EventId,
            evt.CustomerId,
            message,
            "received",
            context.CancellationToken);
    }
}
