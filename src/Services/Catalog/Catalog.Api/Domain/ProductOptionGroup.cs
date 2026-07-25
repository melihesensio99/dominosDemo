namespace Catalog.Api.Domain;

public sealed class ProductOptionGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string SelectionType { get; set; } = "single";

    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();
}
