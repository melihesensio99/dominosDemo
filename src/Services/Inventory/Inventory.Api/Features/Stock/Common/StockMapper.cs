namespace Inventory.Api.Features.Stock.Common;

public static class StockMapper
{
    public static StockResponse ToResponse(StockItem stockItem) =>
        new(
            stockItem.Id,
            stockItem.ProductId,
            stockItem.Available,
            stockItem.Reserved,
            stockItem.ReorderLevel,
            stockItem.Available <= stockItem.ReorderLevel,
            stockItem.CreatedAt,
            stockItem.UpdatedAt);
}
