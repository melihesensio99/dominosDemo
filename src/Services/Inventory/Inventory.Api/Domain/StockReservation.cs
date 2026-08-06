namespace Inventory.Api.Domain;

public sealed class StockReservation
{
    public string OrderId { get; set; } = string.Empty;

    public StockReservationStatus Status { get; set; } = StockReservationStatus.Active;

    public string ItemsJson { get; set; } = "[]";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}

public enum StockReservationStatus
{
    Active = 0,
    Released = 1,
    Committed = 2,
}

public sealed record ReservedStockItem(string StockKey, int Quantity);
