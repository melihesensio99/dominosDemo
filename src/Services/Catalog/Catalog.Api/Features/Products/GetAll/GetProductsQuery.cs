using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record GetProductsQuery : IRequest<Result<IReadOnlyList<ProductResponse>>>;
