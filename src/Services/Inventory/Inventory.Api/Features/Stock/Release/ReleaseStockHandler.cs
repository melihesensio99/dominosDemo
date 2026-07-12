using MassTransit;
using Inventory.Contracts.IntegrationEvents.Inventory;
using Inventory.Api.Features.Stock.Common;

namespace Inventory.Api.Features.Stock.Release;

public sealed class ReleaseStockHandler(
    IStockRepository stockRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<ReleaseStockCommand, Result<StockResponse>>
{
    public async Task<Result<StockResponse>> Handle(ReleaseStockCommand request, CancellationToken cancellationToken)
    {
        var stockItem = await stockRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (stockItem is null)
        {
            return Result<StockResponse>.NotFound("inventory.stock_not_found", "Stock item was not found.");
        }

        if (stockItem.Reserved < request.Quantity)
        {
            return Result<StockResponse>.Conflict("inventory.insufficient_reserved_stock", "Not enough reserved stock available.");
        }

        stockItem.Available += request.Quantity;
        stockItem.Reserved -= request.Quantity;
        stockItem.UpdatedAt = DateTimeOffset.UtcNow;

        await stockRepository.UpdateAsync(stockItem, cancellationToken);
        await publishEndpoint.Publish(
            new StockChangedIntegrationEvent(
                stockItem.ProductId,
                request.Quantity,
                stockItem.Available,
                stockItem.Reserved,
                stockItem.ReorderLevel,
                StockOperationType.Released),
            cancellationToken);

        return Result<StockResponse>.Success(StockMapper.ToResponse(stockItem));
    }
}
