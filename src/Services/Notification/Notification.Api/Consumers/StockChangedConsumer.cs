using MassTransit;
using Inventory.Contracts.IntegrationEvents.Inventory;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class StockChangedConsumer(MongoNotificationStore store) : IConsumer<StockChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<StockChangedIntegrationEvent> context)
    {
        var evt = context.Message;
        var direction = evt.OperationType switch
        {
            StockOperationType.Reserved => "reserved",
            StockOperationType.Released => "released",
            StockOperationType.Adjusted => "adjusted",
            _ => "updated",
        };

        var message = $"Stock for product '{evt.ProductId}' was {direction}. Change: {evt.Quantity}. Available: {evt.Available}, Reserved: {evt.Reserved}.";
        await store.AddAsync(
            evt.EventId,
            "inventory",
            message,
            "received",
            context.CancellationToken);
    }
}
