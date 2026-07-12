namespace Inventory.Api.Features.Stock.Common;

public sealed record StockResponse(
    Guid Id,
    string ProductId,
    int Available,
    int Reserved,
    int ReorderLevel,
    bool IsLowStock,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
