using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    bool IsActive,
    string? ImageUrl = null) : IRequest<Result<ProductResponse>>;
