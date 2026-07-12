namespace Inventory.Api.Domain;

public sealed class StockItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ProductId { get; set; } = string.Empty;

    public int Available { get; set; }

    public int Reserved { get; set; }

    public int ReorderLevel { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
