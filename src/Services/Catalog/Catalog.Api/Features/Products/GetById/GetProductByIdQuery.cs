using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductResponse>>;
