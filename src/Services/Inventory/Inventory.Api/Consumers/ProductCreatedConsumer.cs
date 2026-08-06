using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class ProductCreatedConsumer(IStockRepository stockRepository)
    : IConsumer<ProductCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        if (message.TrackingType.Equals("dough", StringComparison.OrdinalIgnoreCase))
        {
            if (await stockRepository.GetByStockKeyAsync(message.ProductId, context.CancellationToken) is { } obsoleteProductStock)
            {
                obsoleteProductStock.IsActive = false;
                await stockRepository.UpdateAsync(obsoleteProductStock, context.CancellationToken);
            }

            return;
        }

        var stockKey = string.IsNullOrWhiteSpace(message.StockKey) ? message.ProductId : message.StockKey;
        if (await stockRepository.GetByStockKeyAsync(stockKey, context.CancellationToken) is { } existingStock)
        {
            existingStock.DisplayName = message.ProductName;
            existingStock.TrackingType = InventoryTrackingType.Direct;
            existingStock.ReorderLevel = message.ReorderLevel;
            existingStock.IsActive = true;
            await stockRepository.UpdateAsync(existingStock, context.CancellationToken);
            return;
        }

        await stockRepository.AddAsync(new StockItem
        {
            StockKey = stockKey,
            DisplayName = message.ProductName,
            TrackingType = InventoryTrackingType.Direct,
            Available = message.InitialStock,
            Reserved = 0,
            ReorderLevel = message.ReorderLevel,
        }, context.CancellationToken);
    }
}
