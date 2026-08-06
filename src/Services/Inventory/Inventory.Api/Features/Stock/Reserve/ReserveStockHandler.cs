using MassTransit;
using Inventory.Contracts.IntegrationEvents.Inventory;
using Inventory.Api.Features.Stock.Common;

namespace Inventory.Api.Features.Stock.Reserve;

public sealed class ReserveStockHandler(
    IStockRepository stockRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<ReserveStockCommand, Result<StockResponse>>
{
    public async Task<Result<StockResponse>> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var stockItem = await stockRepository.GetByStockKeyAsync(request.ProductId, cancellationToken);
        if (stockItem is null)
        {
            return Result<StockResponse>.NotFound("inventory.stock_not_found", "Stock item was not found.");
        }

        if (stockItem.Available < request.Quantity)
        {
            return Result<StockResponse>.Conflict("inventory.insufficient_stock", "Not enough stock available.");
        }

        stockItem.Available -= request.Quantity;
        stockItem.Reserved += request.Quantity;
        stockItem.UpdatedAt = DateTimeOffset.UtcNow;

        await stockRepository.UpdateAsync(stockItem, cancellationToken);
        await publishEndpoint.Publish(
            new StockChangedIntegrationEvent(
                stockItem.StockKey,
                request.Quantity,
                stockItem.Available,
                stockItem.Reserved,
                stockItem.ReorderLevel,
                StockOperationType.Reserved),
            cancellationToken);

        return Result<StockResponse>.Success(StockMapper.ToResponse(stockItem));
    }
}
