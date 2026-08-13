using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class ProductUpdatedConsumer(IStockRepository stockRepository)
    : IConsumer<ProductUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductUpdatedIntegrationEvent> context)
    {
        var message = context.Message;

        if (message.TrackingType.Equals("dough", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var stockKey = string.IsNullOrWhiteSpace(message.StockKey) ? message.ProductId : message.StockKey;
        if (await stockRepository.GetByStockKeyAsync(stockKey, context.CancellationToken) is not { } existingStock)
        {
            await stockRepository.AddAsync(new StockItem
            {
                StockKey = stockKey,
                DisplayName = message.ProductName,
                TrackingType = InventoryTrackingType.Direct,
                Available = message.Stock,
                Reserved = 0,
                ReorderLevel = message.ReorderLevel,
                IsActive = message.IsActive,
            }, context.CancellationToken);
            return;
        }

        existingStock.DisplayName = message.ProductName;
        existingStock.IsActive = message.IsActive;
        existingStock.ReorderLevel = message.ReorderLevel;
        existingStock.UpdatedAt = DateTimeOffset.UtcNow;
        existingStock.Available = message.Stock;

        if (!message.IsActive)
        {
            existingStock.LowStockNotified = false;
        }

        await stockRepository.UpdateAsync(existingStock, context.CancellationToken);
    }
}
