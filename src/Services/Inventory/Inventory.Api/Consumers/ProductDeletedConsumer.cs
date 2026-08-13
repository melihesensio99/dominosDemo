using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class ProductDeletedConsumer(IStockRepository stockRepository)
    : IConsumer<ProductDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context)
    {
        var message = context.Message;
        var stockKey = string.IsNullOrWhiteSpace(message.StockKey) ? message.ProductId : message.StockKey;

        if (await stockRepository.GetByStockKeyAsync(stockKey, context.CancellationToken) is not { } existingStock)
        {
            return;
        }

        existingStock.IsActive = false;
        existingStock.LowStockNotified = false;
        existingStock.UpdatedAt = DateTimeOffset.UtcNow;
        await stockRepository.UpdateAsync(existingStock, context.CancellationToken);
    }
}
