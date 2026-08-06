namespace Notification.Api.Abstractions.Realtime;

public sealed record NewOrderNotification(
    string OrderId,
    string CustomerId,
    string Status,
    int ItemCount,
    DateTimeOffset CreatedAt);

public sealed record OrderStatusChangedNotification(
    string OrderId,
    string CustomerId,
    string Status,
    DateTimeOffset UpdatedAt);

public sealed record LowStockNotification(
    string Id,
    string StockKey,
    string DisplayName,
    int Available,
    int ReorderLevel,
    DateTimeOffset CreatedAt);
