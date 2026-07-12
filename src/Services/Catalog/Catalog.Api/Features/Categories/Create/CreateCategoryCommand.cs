using MediatR;

namespace Catalog.Api.Features.Categories;

public sealed record CreateCategoryCommand(string Name) : IRequest<Result<CategoryResponse>>;
