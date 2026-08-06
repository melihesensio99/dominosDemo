namespace Inventory.Api.Features.Stock.Common;

public sealed record StockResponse(
    Guid Id,
    string StockKey,
    string DisplayName,
    InventoryTrackingType TrackingType,
    int Available,
    int Reserved,
    int ReorderLevel,
    bool IsLowStock,
    bool LowStockNotified,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
