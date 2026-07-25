using Inventory.Contracts.IntegrationEvents.Catalog;
using MassTransit;

namespace Inventory.Api.Consumers;

public sealed class ProductCreatedConsumer(IStockRepository stockRepository)
    : IConsumer<ProductCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        if (await stockRepository.GetByProductIdAsync(message.ProductId, context.CancellationToken) is not null)
        {
            return;
        }

        await stockRepository.AddAsync(new StockItem
        {
            ProductId = message.ProductId,
            Available = message.InitialStock,
            Reserved = 0,
            ReorderLevel = message.ReorderLevel,
        }, context.CancellationToken);
    }
}
