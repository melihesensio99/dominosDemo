namespace Inventory.Api.Features.Stock.Common;

public static class StockMapper
{
    public static StockResponse ToResponse(StockItem stockItem) =>
        new(
            stockItem.Id,
            stockItem.StockKey,
            stockItem.DisplayName,
            stockItem.TrackingType,
            stockItem.Available,
            stockItem.Reserved,
            stockItem.ReorderLevel,
            stockItem.Available <= stockItem.ReorderLevel,
            stockItem.LowStockNotified,
            stockItem.CreatedAt,
            stockItem.UpdatedAt);
}
