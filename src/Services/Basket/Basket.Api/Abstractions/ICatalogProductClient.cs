namespace Basket.Api.Abstractions;

public interface ICatalogProductClient
{
    Task<Result<CatalogProductSnapshot>> GetProductAsync(string productId, CancellationToken cancellationToken);
}

public sealed record CatalogProductSnapshot(
    string Id,
    string Name,
    decimal Price,
    bool IsActive,
    string InventoryTrackingType,
    string? InventoryKey,
    IReadOnlyList<CatalogOptionGroupSnapshot> OptionGroups);

public sealed record CatalogOptionGroupSnapshot(
    Guid Id,
    string Name,
    string SelectionType,
    bool IsRequired,
    IReadOnlyList<CatalogOptionSnapshot> Options);

public sealed record CatalogOptionSnapshot(
    Guid Id,
    string GroupName,
    string Name,
    decimal PriceAdjustment,
    string? InventoryKey,
    bool IsDefault,
    bool IsActive);
