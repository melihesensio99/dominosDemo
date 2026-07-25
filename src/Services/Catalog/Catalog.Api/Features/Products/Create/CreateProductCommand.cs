using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    int ReorderLevel = 5,
    IReadOnlyList<CreateProductOptionGroup>? OptionGroups = null) : IRequest<Result<ProductResponse>>;

public sealed record CreateProductOptionGroup(
    string Name,
    string SelectionType,
    bool IsRequired,
    IReadOnlyList<CreateProductOption> Options,
    int DisplayOrder = 0);

public sealed record CreateProductOption(
    string Name,
    decimal PriceAdjustment,
    int DisplayOrder = 0);
