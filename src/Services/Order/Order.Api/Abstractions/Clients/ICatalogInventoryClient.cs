namespace Order.Api.Abstractions;

public interface ICatalogInventoryClient
{
    Task<CatalogInventoryProduct?> GetProductAsync(string productId, CancellationToken cancellationToken);
}

public sealed record CatalogInventoryProduct(
    string Id,
    string InventoryTrackingType,
    string? InventoryKey,
    bool IsActive,
    decimal Price,
    IReadOnlyList<CatalogInventoryOption> Options);

public sealed record CatalogInventoryOption(Guid Id, string? InventoryKey, bool IsActive, decimal PriceAdjustment);
