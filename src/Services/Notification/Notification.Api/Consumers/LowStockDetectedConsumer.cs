using Inventory.Contracts.IntegrationEvents.Inventory;
using MassTransit;
using Notification.Api.Abstractions.Realtime;
using Notification.Api.Infrastructure;

namespace Notification.Api.Consumers;

public sealed class LowStockDetectedConsumer(
    MongoNotificationStore store,
    IRealtimeNotificationPublisher realtimePublisher)
    : IConsumer<LowStockDetectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<LowStockDetectedIntegrationEvent> context)
    {
        var message = context.Message;
        var (document, isNew) = await store.AddLowStockAsync(
            message.EventId,
            message.StockKey,
            message.DisplayName,
            message.Available,
            message.ReorderLevel,
            context.CancellationToken);

        if (isNew)
        {
            await realtimePublisher.NotifyLowStockAsync(new LowStockNotification(
                document.Id,
                message.StockKey,
                message.DisplayName,
                message.Available,
                message.ReorderLevel,
                document.CreatedAt));
        }
    }
}
