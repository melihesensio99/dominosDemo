using Inventory.Contracts.IntegrationEvents.Order;
using MassTransit;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class OrderCreatedConsumer(MongoNotificationStore store) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        var message = $"Order '{evt.OrderId}' was created for customer '{evt.CustomerId}' with {evt.ItemCount} items.";
        await store.AddAsync(evt.CustomerId, message, "received", context.CancellationToken);
    }
}
