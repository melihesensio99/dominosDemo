using MassTransit;
using Inventory.Contracts.IntegrationEvents.Inventory;

namespace Notification.Api.Consumers;

public sealed class StockChangedConsumer(NotificationStore store) : IConsumer<StockChangedIntegrationEvent>
{
    public Task Consume(ConsumeContext<StockChangedIntegrationEvent> context)
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
        store.Add("inventory", message, "received");

        return Task.CompletedTask;
    }
}
