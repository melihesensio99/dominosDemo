namespace Catalog.Api.Domain;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public InventoryTrackingType InventoryTrackingType { get; set; } = InventoryTrackingType.Direct;

    public string? InventoryKey { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<ProductOptionGroup> OptionGroups { get; set; } = new List<ProductOptionGroup>();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}
