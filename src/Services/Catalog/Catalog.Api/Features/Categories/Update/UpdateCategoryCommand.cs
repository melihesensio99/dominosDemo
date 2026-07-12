using MediatR;

namespace Catalog.Api.Features.Categories;

public sealed record UpdateCategoryCommand(Guid Id, string Name, bool IsActive) : IRequest<Result<CategoryResponse>>;
