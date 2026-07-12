using MediatR;

namespace Catalog.Api.Features.Products;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Result>;
