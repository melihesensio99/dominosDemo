using MediatR;

namespace Catalog.Api.Features.Categories;

public sealed record CategoryDetailsQuery(Guid Id) : IRequest<Result<CategoryResponse>>;
