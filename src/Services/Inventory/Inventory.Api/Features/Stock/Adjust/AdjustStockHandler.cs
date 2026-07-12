using MassTransit;
using Inventory.Contracts.IntegrationEvents.Inventory;
using Inventory.Api.Features.Stock.Common;

namespace Inventory.Api.Features.Stock.Adjust;

public sealed class AdjustStockHandler(
    IStockRepository stockRepository,
    IPublishEndpoint publishEndpoint) : IRequestHandler<AdjustStockCommand, Result<StockResponse>>
{
    public async Task<Result<StockResponse>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var stockItem = await stockRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (stockItem is null)
        {
            return Result<StockResponse>.NotFound("inventory.stock_not_found", "Stock item was not found.");
        }

        var newAvailable = stockItem.Available + request.Quantity;
        if (newAvailable < 0)
        {
            return Result<StockResponse>.Conflict("inventory.stock_cannot_go_below_zero", "Stock cannot be reduced below zero.");
        }

        stockItem.Available = newAvailable;
        stockItem.UpdatedAt = DateTimeOffset.UtcNow;

        await stockRepository.UpdateAsync(stockItem, cancellationToken);
        await publishEndpoint.Publish(
            new StockChangedIntegrationEvent(
                stockItem.ProductId,
                request.Quantity,
                stockItem.Available,
                stockItem.Reserved,
                stockItem.ReorderLevel,
                StockOperationType.Adjusted),
            cancellationToken);

        return Result<StockResponse>.Success(StockMapper.ToResponse(stockItem));
    }
}
