using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId) : IRequest<Result<ProductResponse>>;
