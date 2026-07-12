using MediatR;

namespace Catalog.Api.Features.Categories;

public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryResponse>>>;
