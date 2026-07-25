namespace Catalog.Api.Features.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string? CategoryName,
    IReadOnlyList<ProductOptionGroupResponse> OptionGroups,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record ProductOptionGroupResponse(
    Guid Id,
    string Name,
    string SelectionType,
    bool IsRequired,
    int DisplayOrder,
    IReadOnlyList<ProductOptionResponse> Options);

public sealed record ProductOptionResponse(
    Guid Id,
    string Name,
    decimal PriceAdjustment,
    bool IsActive,
    int DisplayOrder);
