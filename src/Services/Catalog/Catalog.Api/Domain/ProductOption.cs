namespace Catalog.Api.Domain;

public sealed class ProductOption
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductOptionGroupId { get; set; }

    public ProductOptionGroup ProductOptionGroup { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal PriceAdjustment { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}
