namespace Inventory.Api.Domain;

public sealed class StockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string StockKey { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public InventoryTrackingType TrackingType { get; set; }

    public int Available { get; set; }

    public int Reserved { get; set; }

    public int ReorderLevel { get; set; }

    public bool LowStockNotified { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
