namespace Basket.Api.Infrastructure.Clients.Catalog.Contracts;

internal sealed record CatalogProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    bool IsActive,
    string InventoryTrackingType,
    string? InventoryKey,
    IReadOnlyList<CatalogOptionGroupResponse> OptionGroups);

internal sealed record CatalogOptionGroupResponse(
    Guid Id,
    string Name,
    string SelectionType,
    bool IsRequired,
    IReadOnlyList<CatalogOptionResponse> Options);

internal sealed record CatalogOptionResponse(
    Guid Id,
    string Name,
    decimal PriceAdjustment,
    string? InventoryKey,
    bool IsDefault,
    bool IsActive,
    int DisplayOrder);
